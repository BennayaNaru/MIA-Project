namespace OpenSch.Application.Services;

/// <summary>
/// Canonical source for the synthetic school data used by the API.
/// Keeping this on the server prevents a browser from changing values before
/// they are passed to the comparison service or an AI provider.
/// </summary>
public static class SchoolCatalog
{
    public static readonly IReadOnlyList<SchoolRecord> All =
    [
        new("DKI-01", "SD Negeri Menteng Atas 01 Sintetis", "Jakarta Selatan", "Tebet", "SD", 84.5m, 81.2m),
        new("DKI-02", "SMP Cendekia Jaya Mandiri", "Jakarta Selatan", "Cilandak", "SMP", 88.0m, 86.5m),
        new("DKI-03", "SMK Teknologi Bangsa Pratama", "Jakarta Selatan", "Kebayoran Baru", "SMK", 79.4m, 85.0m),
        new("DKI-04", "SMP Teladan Jagakarsa Mandiri", "Jakarta Selatan", "Jagakarsa", "SMP", 83.0m, 81.5m),
        new("DKI-05", "SD Harapan Bangsa Cakung", "Jakarta Timur", "Cakung", "SD", 80.2m, 76.8m),
        new("DKI-06", "SMP Negeri Rawamangun Maju", "Jakarta Timur", "Duren Sawit", "SMP", 82.6m, 79.5m),
        new("DKI-07", "SMK Karya Mandiri Otomotif", "Jakarta Timur", "Jatinegara", "SMK", 76.5m, 88.2m),
        new("DKI-08", "SMK Insan Mulia Ciracas", "Jakarta Timur", "Ciracas", "SMK", 78.2m, 84.0m),
        new("DKI-09", "SD Pesisir Bintang Bahari", "Jakarta Utara", "Tanjung Priok", "SD", 78.0m, 74.2m),
        new("DKI-10", "SMP Samudera Indah Gading", "Jakarta Utara", "Kelapa Gading", "SMP", 86.4m, 84.0m),
        new("DKI-11", "SMK Maritim & Logistik Perkasa", "Jakarta Utara", "Penjaringan", "SMK", 75.8m, 83.5m),
        new("DKI-12", "SMP Citra Bahari Pademangan", "Jakarta Utara", "Pademangan", "SMP", 81.5m, 78.0m),
        new("DKI-13", "SD Citra Melati Kembangan", "Jakarta Barat", "Kembangan", "SD", 81.5m, 78.0m),
        new("DKI-14", "SMP Prestasi Cemerlang Grogol", "Jakarta Barat", "Grogol Petamburan", "SMP", 83.2m, 80.4m),
        new("DKI-15", "SMK Rekayasa Digital Informatika", "Jakarta Barat", "Kebon Jeruk", "SMK", 82.0m, 89.6m),
        new("DKI-16", "SMK Grafika Cengkareng Unggul", "Jakarta Barat", "Cengkareng", "SMK", 77.0m, 82.5m),
    ];

    public static IReadOnlyList<SchoolDto> GetAll() => All
        .OrderByDescending(s => s.CompositeScore)
        .ThenByDescending(s => s.LiteracyScore)
        .ThenBy(s => s.Id, StringComparer.Ordinal)
        .Select((s, index) => s.ToDto(index + 1))
        .ToList();

    /// <summary>
    /// Resolves a user-supplied list of school IDs against the canonical catalog.
    /// IDs are trimmed and de-duplicated, and the list must contain 2 or 3 schools.
    /// </summary>
    public static bool TryResolve(
        IReadOnlyList<string>? requestedIds,
        out IReadOnlyList<SchoolDto> schools,
        out string? error)
    {
        var ids = requestedIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? [];

        if (ids.Length is < 2 or > 3)
        {
            schools = [];
            error = "Pilih tepat 2 atau 3 sekolah.";
            return false;
        }

        var catalog = GetAll().ToDictionary(s => s.Id, StringComparer.Ordinal);
        var missing = ids.Where(id => !catalog.ContainsKey(id)).ToArray();
        if (missing.Length > 0)
        {
            schools = [];
            error = $"ID sekolah tidak ditemukan: {string.Join(", ", missing)}.";
            return false;
        }

        schools = ids.Select(id => catalog[id]).ToList();
        error = null;
        return true;
    }
}

public sealed record SchoolRecord(
    string Id,
    string Name,
    string City,
    string District,
    string Level,
    decimal LiteracyScore,
    decimal NumeracyScore)
{
    public decimal CompositeScore => decimal.Round((LiteracyScore + NumeracyScore) / 2, 1);

    public SchoolDto ToDto(int rank) => new(Id, Name, City, District, Level, LiteracyScore, NumeracyScore, CompositeScore, rank);
}

public sealed record SchoolDto(
    string Id,
    string Name,
    string City,
    string District,
    string Level,
    decimal LiteracyScore,
    decimal NumeracyScore,
    decimal CompositeScore,
    int Rank);
