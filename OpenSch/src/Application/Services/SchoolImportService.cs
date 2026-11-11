using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using OpenSch.Application.Common.Interfaces;
using OpenSch.Domain.Entities;

namespace OpenSch.Application.Services;

public sealed record ImportRowError(int RowNumber, string Field, string Message);

public sealed record ImportReport(
    long ImportId,
    string FileName,
    string Status,
    int TotalRows,
    int Inserted,
    int Updated,
    IReadOnlyList<ImportRowError> Errors)
{
    public bool Succeeded => Status == SchoolImportService.StatusSucceeded;
}

public interface ISchoolImportService
{
    /// <summary>
    /// Validates and upserts a CSV of schools. The write is atomic: either every row is
    /// applied or none is, so a bad file can never damage the data users see.
    /// </summary>
    Task<ImportReport> ImportAsync(
        Stream csvStream,
        string fileName,
        string? penggunaId,
        CancellationToken cancellationToken = default);
}

public sealed class SchoolImportService(IApplicationDbContext context) : ISchoolImportService
{
    public const string StatusSucceeded = "BERHASIL";
    public const string StatusFailed = "GAGAL";

    private const int MaxReportedErrors = 50;

    private static readonly string[] RequiredColumns = ["npsn", "nama", "jenjang", "wilayah", "literasi", "numerasi"];
    private static readonly string[] ValidLevels = ["SD", "SMP", "SMK"];
    private static readonly string[] ValidStatuses = ["Negeri", "Swasta"];

    public async Task<ImportReport> ImportAsync(
        Stream csvStream,
        string fileName,
        string? penggunaId,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<ImportRowError>();
        var rows = new List<ImportRow>();
        var totalRows = 0;

        using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            HeaderValidated = null,
            BadDataFound = null,
        });

        if (!await csv.ReadAsync())
        {
            errors.Add(new ImportRowError(1, "file", "File CSV kosong."));
            return await SaveReportAsync(fileName, penggunaId, totalRows, 0, 0, errors, "File CSV kosong.", cancellationToken);
        }

        csv.ReadHeader();

        var headers = csv.HeaderRecord ?? [];
        var columnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < headers.Length; index++)
        {
            columnIndex[headers[index].Trim()] = index;
        }

        var missingColumns = RequiredColumns.Where(column => !columnIndex.ContainsKey(column)).ToArray();
        if (missingColumns.Length > 0)
        {
            errors.Add(new ImportRowError(1, "header", $"Kolom wajib tidak ditemukan: {string.Join(", ", missingColumns)}."));
            return await SaveReportAsync(fileName, penggunaId, totalRows, 0, 0, errors, null, cancellationToken);
        }

        var rowNumber = 1;
        var seenNpsn = new HashSet<string>(StringComparer.Ordinal);

        while (await csv.ReadAsync())
        {
            rowNumber++;
            totalRows++;

            var rowErrors = new List<ImportRowError>();
            var row = ParseRow(csv, columnIndex, rowNumber, seenNpsn, rowErrors);

            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            rows.Add(row!);
        }

        if (totalRows == 0)
        {
            errors.Add(new ImportRowError(1, "file", "File CSV tidak memiliki baris data."));
            return await SaveReportAsync(fileName, penggunaId, totalRows, 0, 0, errors, null, cancellationToken);
        }

        if (errors.Count > 0)
        {
            return await SaveReportAsync(fileName, penggunaId, totalRows, 0, 0, errors, null, cancellationToken);
        }

        var npsns = rows.Select(row => row.Npsn).ToArray();
        var existing = await context.Sekolah
            .Where(school => school.Npsn != null && npsns.Contains(school.Npsn))
            .ToDictionaryAsync(school => school.Npsn!, StringComparer.Ordinal, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var inserted = 0;
        var updated = 0;

        foreach (var row in rows)
        {
            if (existing.TryGetValue(row.Npsn, out var school))
            {
                Apply(row, school, now);
                updated++;
            }
            else
            {
                context.Sekolah.Add(CreateEntity(row, now));
                inserted++;
            }
        }

        // One SaveChanges call keeps the school upserts and the import log atomic.
        return await SaveReportAsync(
            fileName,
            penggunaId,
            totalRows,
            inserted,
            updated,
            errors,
            null,
            cancellationToken,
            new ImportLogDetails(StatusSucceeded, true, null));
    }

    private ImportRow? ParseRow(
        IReaderRow csv,
        IReadOnlyDictionary<string, int> columns,
        int rowNumber,
        ISet<string> seenNpsn,
        ICollection<ImportRowError> errors)
    {
        var npsn = Value(csv, columns, "npsn");
        var nama = Value(csv, columns, "nama");
        var level = Value(csv, columns, "jenjang").ToUpperInvariant();
        var wilayah = Value(csv, columns, "wilayah");
        var kecamatan = Value(csv, columns, "kecamatan");
        var status = Value(csv, columns, "status");
        var literacy = Value(csv, columns, "literasi");
        var numeracy = Value(csv, columns, "numerasi");
        var students = Value(csv, columns, "murid");
        var teachers = Value(csv, columns, "guru");
        var classrooms = Value(csv, columns, "ruang_kelas");
        var dataDate = Value(csv, columns, "tanggal_data");

        if (string.IsNullOrWhiteSpace(npsn) || !npsn.All(char.IsAsciiDigit) || npsn.Length != 8)
        {
            errors.Add(new ImportRowError(rowNumber, "npsn", "NPSN wajib diisi dengan 8 digit angka."));
        }
        else if (!seenNpsn.Add(npsn))
        {
            errors.Add(new ImportRowError(rowNumber, "npsn", $"NPSN {npsn} muncul lebih dari sekali di file."));
        }

        if (string.IsNullOrWhiteSpace(nama))
        {
            errors.Add(new ImportRowError(rowNumber, "nama", "Nama sekolah wajib diisi."));
        }
        else if (nama.Length > 255)
        {
            errors.Add(new ImportRowError(rowNumber, "nama", "Nama sekolah maksimal 255 karakter."));
        }

        if (!ValidLevels.Contains(level))
        {
            errors.Add(new ImportRowError(rowNumber, "jenjang", "Jenjang harus salah satu dari: SD, SMP, SMK."));
        }

        if (string.IsNullOrWhiteSpace(wilayah))
        {
            errors.Add(new ImportRowError(rowNumber, "wilayah", "Wilayah wajib diisi."));
        }

        if (!string.IsNullOrWhiteSpace(status) && !ValidStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(new ImportRowError(rowNumber, "status", "Status harus Negeri atau Swasta."));
        }

        var literacyScore = ParseScore(csv, columns, "literasi", rowNumber, errors);
        var numeracyScore = ParseScore(csv, columns, "numerasi", rowNumber, errors);

        var studentCount = ParseCount(csv, columns, "murid", rowNumber, errors);
        var teacherCount = ParseCount(csv, columns, "guru", rowNumber, errors);
        var classroomCount = ParseCount(csv, columns, "ruang_kelas", rowNumber, errors);

        var dataDateValue = DateOnly.FromDateTime(DateTime.UtcNow);
        if (!string.IsNullOrWhiteSpace(dataDate) &&
            !DateOnly.TryParse(dataDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataDateValue))
        {
            errors.Add(new ImportRowError(rowNumber, "tanggal_data", "Tanggal data harus berformat yyyy-MM-dd."));
        }

        if (errors.Count > 0)
        {
            return null;
        }

        return new ImportRow(
            npsn,
            nama,
            wilayah,
            string.IsNullOrWhiteSpace(kecamatan) ? wilayah : kecamatan,
            level,
            string.IsNullOrWhiteSpace(status) ? "Negeri" : status,
            literacyScore!.Value,
            numeracyScore!.Value,
            studentCount,
            teacherCount,
            classroomCount,
            dataDateValue);
    }

    private static decimal? ParseScore(
        IReaderRow csv,
        IReadOnlyDictionary<string, int> columns,
        string column,
        int rowNumber,
        ICollection<ImportRowError> errors)
    {
        var raw = Value(csv, columns, column);
        if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var value) || value is < 0m or > 100m)
        {
            errors.Add(new ImportRowError(rowNumber, column, "Skor harus berupa angka antara 0 dan 100."));
            return null;
        }

        return value;
    }

    private static int? ParseCount(
        IReaderRow csv,
        IReadOnlyDictionary<string, int> columns,
        string column,
        int rowNumber,
        ICollection<ImportRowError> errors)
    {
        var raw = Value(csv, columns, column);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value < 0)
        {
            errors.Add(new ImportRowError(rowNumber, column, "Nilai harus berupa bilangan bulat positif."));
            return null;
        }

        return value;
    }

    private static string Value(IReaderRow csv, IReadOnlyDictionary<string, int> columns, string column) =>
        columns.TryGetValue(column, out var index) ? (csv.GetField(index) ?? string.Empty).Trim() : string.Empty;

    private static void Apply(ImportRow row, Sekolah school, DateTimeOffset now)
    {
        school.Nama = row.Nama;
        school.Kota = row.Wilayah;
        school.KabupatenKota = row.Kecamatan;
        school.Level = row.Level;
        school.Status = row.Status;
        school.JumlahMurid = row.Murid;
        school.JumlahGuru = row.Guru;
        school.RuangKelas = row.RuangKelas;
        school.SkorMelekHuruf = row.Literacy;
        school.SkorMelekAngka = row.Numeracy;
        school.SkorKomposit = decimal.Round((row.Literacy + row.Numeracy) / 2m, 1);
        school.TanggalData = row.TanggalData;
        school.UpdatedAt = now;
    }

    private static Sekolah CreateEntity(ImportRow row, DateTimeOffset now) => new()
    {
        Id = row.Npsn,
        Npsn = row.Npsn,
        Nama = row.Nama,
        Kota = row.Wilayah,
        KabupatenKota = row.Kecamatan,
        Level = row.Level,
        Status = row.Status,
        JumlahMurid = row.Murid,
        JumlahGuru = row.Guru,
        RuangKelas = row.RuangKelas,
        SkorMelekHuruf = row.Literacy,
        SkorMelekAngka = row.Numeracy,
        SkorKomposit = decimal.Round((row.Literacy + row.Numeracy) / 2m, 1),
        TanggalData = row.TanggalData,
        CreatedAt = now,
        UpdatedAt = now,
    };

    private async Task<ImportReport> SaveReportAsync(
        string fileName,
        string? penggunaId,
        int totalRows,
        int inserted,
        int updated,
        IReadOnlyList<ImportRowError> errors,
        string? message,
        CancellationToken cancellationToken,
        ImportLogDetails? details = null)
    {
        var succeeded = details?.Succeeded ?? false;
        var status = details?.Status ?? StatusFailed;

        var import = new ImporData
        {
            PenggunaId = penggunaId,
            NamaFile = fileName,
            StatusImpor = status,
            TanggalImpor = DateTimeOffset.UtcNow,
            JumlahBaris = totalRows,
            JumlahBerhasil = succeeded ? inserted + updated : 0,
            JumlahGagal = errors.Count,
            Sukses = succeeded,
            Pesan = message ?? SummarizeErrors(errors),
        };

        context.ImporData.Add(import);
        await context.SaveChangesAsync(cancellationToken);

        return new ImportReport(
            import.Id,
            fileName,
            status,
            totalRows,
            inserted,
            updated,
            errors.Take(MaxReportedErrors).ToList());
    }

    private static string? SummarizeErrors(IReadOnlyList<ImportRowError> errors)
    {
        if (errors.Count == 0)
        {
            return null;
        }

        var summary = string.Join(" | ", errors.Take(5).Select(error => $"baris {error.RowNumber} {error.Field}: {error.Message}"));
        if (errors.Count > 5)
        {
            summary += $" | (+{errors.Count - 5} kesalahan lain)";
        }

        return summary.Length > 2000 ? summary[..2000] : summary;
    }

    private sealed record ImportRow(
        string Npsn,
        string Nama,
        string Wilayah,
        string Kecamatan,
        string Level,
        string Status,
        decimal Literacy,
        decimal Numeracy,
        int? Murid,
        int? Guru,
        int? RuangKelas,
        DateOnly TanggalData);

    private sealed record ImportLogDetails(string Status, bool Succeeded, string? Message);
}
