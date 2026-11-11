namespace OpenSch.Application.Services;

/// <summary>
/// Canonical source for the synthetic school data used to seed a fresh database.
/// Keeping this on the server prevents a browser from changing values before
/// they are passed to the comparison service or an AI provider.
/// </summary>
public static class SchoolCatalog
{
    public static readonly IReadOnlyList<SchoolRecord> All =
    [
        S(1, "SD Negeri Menteng Atas 01 Sintetis", "Jakarta Selatan", "Tebet", "SD", 84.5m, 81.2m, "Negeri"),
        S(2, "SMP Cendekia Jaya Mandiri", "Jakarta Selatan", "Cilandak", "SMP", 88.0m, 86.5m, "Swasta"),
        S(3, "SMK Teknologi Bangsa Pratama", "Jakarta Selatan", "Kebayoran Baru", "SMK", 79.4m, 85.0m, "Swasta"),
        S(4, "SMP Teladan Jagakarsa Mandiri", "Jakarta Selatan", "Jagakarsa", "SMP", 83.0m, 81.5m, "Swasta"),
        S(5, "SD Harapan Bangsa Cakung", "Jakarta Timur", "Cakung", "SD", 80.2m, 76.8m, "Swasta"),
        S(6, "SMP Negeri Rawamangun Maju", "Jakarta Timur", "Duren Sawit", "SMP", 82.6m, 79.5m, "Negeri"),
        S(7, "SMK Karya Mandiri Otomotif", "Jakarta Timur", "Jatinegara", "SMK", 76.5m, 88.2m, "Swasta"),
        S(8, "SMK Insan Mulia Ciracas", "Jakarta Timur", "Ciracas", "SMK", 78.2m, 84.0m, "Swasta"),
        S(9, "SD Pesisir Bintang Bahari", "Jakarta Utara", "Tanjung Priok", "SD", 78.0m, 74.2m, "Swasta"),
        S(10, "SMP Samudera Indah Gading", "Jakarta Utara", "Kelapa Gading", "SMP", 86.4m, 84.0m, "Swasta"),
        S(11, "SMK Maritim & Logistik Perkasa", "Jakarta Utara", "Penjaringan", "SMK", 75.8m, 83.5m, "Swasta"),
        S(12, "SMP Citra Bahari Pademangan", "Jakarta Utara", "Pademangan", "SMP", 81.5m, 78.0m, "Swasta"),
        S(13, "SD Citra Melati Kembangan", "Jakarta Barat", "Kembangan", "SD", 81.5m, 78.0m, "Swasta"),
        S(14, "SMP Prestasi Cemerlang Grogol", "Jakarta Barat", "Grogol Petamburan", "SMP", 83.2m, 80.4m, "Swasta"),
        S(15, "SMK Rekayasa Digital Informatika", "Jakarta Barat", "Kebon Jeruk", "SMK", 82.0m, 89.6m, "Swasta"),
        S(16, "SMK Grafika Cengkareng Unggul", "Jakarta Barat", "Cengkareng", "SMK", 77.0m, 82.5m, "Swasta"),
        S(17, "SD Negeri Kramat Pela 03", "Jakarta Selatan", "Kebayoran Baru", "SD", 86.1m, 82.4m, "Negeri"),
        S(18, "SMP Muhammadiyah Cipete", "Jakarta Selatan", "Cilandak", "SMP", 81.8m, 79.6m, "Swasta"),
        S(19, "SMK Bina Informatika Pasar Minggu", "Jakarta Selatan", "Pasar Minggu", "SMK", 78.9m, 86.7m, "Swasta"),
        S(20, "SD Islam Al-Azhar Mampang", "Jakarta Selatan", "Mampang Prapatan", "SD", 87.3m, 84.9m, "Swasta"),
        S(21, "SMP Negeri 21 Tebet", "Jakarta Selatan", "Tebet", "SMP", 85.2m, 83.1m, "Negeri"),
        S(22, "SMK Kesehatan Setiabudi", "Jakarta Selatan", "Setiabudi", "SMK", 80.6m, 81.9m, "Swasta"),
        S(23, "SD Negeri Cipinang Besar 07", "Jakarta Timur", "Jatinegara", "SD", 79.8m, 77.4m, "Negeri"),
        S(24, "SMP Barunawati Duren Sawit", "Jakarta Timur", "Duren Sawit", "SMP", 82.1m, 80.9m, "Swasta"),
        S(25, "SMK Tunas Muda Cakung", "Jakarta Timur", "Cakung", "SMK", 77.4m, 84.2m, "Swasta"),
        S(26, "SD Negeri Pulogadung 05", "Jakarta Timur", "Pulogadung", "SD", 83.4m, 79.1m, "Negeri"),
        S(27, "SMP Negeri 87 Ciracas", "Jakarta Timur", "Ciracas", "SMP", 84.7m, 82.6m, "Negeri"),
        S(28, "SMK Multimedia Rawamangun", "Jakarta Timur", "Pulogadung", "SMK", 81.3m, 87.5m, "Swasta"),
        S(29, "SD Pelita Gading", "Jakarta Utara", "Kelapa Gading", "SD", 85.9m, 82.8m, "Swasta"),
        S(30, "SMP Negeri 30 Penjaringan", "Jakarta Utara", "Penjaringan", "SMP", 80.7m, 78.3m, "Negeri"),
        S(31, "SMK Bahari Tanjung Priok", "Jakarta Utara", "Tanjung Priok", "SMK", 76.8m, 82.1m, "Swasta"),
        S(32, "SD Negeri Koja 09", "Jakarta Utara", "Koja", "SD", 78.6m, 75.9m, "Negeri"),
        S(33, "SMP Cinta Kasih Cilincing", "Jakarta Utara", "Cilincing", "SMP", 81.2m, 79.8m, "Swasta"),
        S(34, "SMK Penerbangan Kemayoran", "Jakarta Pusat", "Kemayoran", "SMK", 83.7m, 88.0m, "Swasta"),
        S(35, "SD Negeri Menteng 02", "Jakarta Pusat", "Menteng", "SD", 88.4m, 86.2m, "Negeri"),
        S(36, "SMP Negeri 5 Tanah Abang", "Jakarta Pusat", "Tanah Abang", "SMP", 82.9m, 81.4m, "Negeri"),
        S(37, "SMK Grafika Sawah Besar", "Jakarta Pusat", "Sawah Besar", "SMK", 79.1m, 83.6m, "Swasta"),
        S(38, "SD Santo Yosef Senen", "Jakarta Pusat", "Senen", "SD", 84.2m, 82.7m, "Swasta"),
        S(39, "SMP Labschool Cempaka Putih", "Jakarta Pusat", "Cempaka Putih", "SMP", 89.1m, 87.3m, "Swasta"),
        S(40, "SMK Bina Niaga Johar Baru", "Jakarta Pusat", "Johar Baru", "SMK", 78.3m, 80.5m, "Swasta"),
        S(41, "SD Negeri Grogol 04", "Jakarta Barat", "Grogol Petamburan", "SD", 80.1m, 76.7m, "Negeri"),
        S(42, "SMP Negeri 45 Kebon Jeruk", "Jakarta Barat", "Kebon Jeruk", "SMP", 84.6m, 83.9m, "Negeri"),
        S(43, "SMK Pariwisata Palmerah", "Jakarta Barat", "Palmerah", "SMK", 77.9m, 81.2m, "Swasta"),
        S(44, "SD Taman Sari 01", "Jakarta Barat", "Taman Sari", "SD", 79.4m, 77.1m, "Negeri"),
        S(45, "SMP Kasih Bunda Cengkareng", "Jakarta Barat", "Cengkareng", "SMP", 81.7m, 79.9m, "Swasta"),
        S(46, "SMK Otomotif Kalideres", "Jakarta Barat", "Kalideres", "SMK", 75.6m, 85.4m, "Swasta"),
        S(47, "SD Negeri Joglo 06", "Jakarta Barat", "Kembangan", "SD", 82.8m, 80.3m, "Negeri"),
        S(48, "SMP Islam Al-Hikmah Kebayoran", "Jakarta Selatan", "Kebayoran Lama", "SMP", 83.9m, 82.0m, "Swasta"),
        S(49, "SMK Nusantara Kembangan", "Jakarta Barat", "Kembangan", "SMK", 79.7m, 84.8m, "Swasta"),
        S(50, "SD Negeri Cakung Timur 02", "Jakarta Timur", "Cakung", "SD", 81.1m, 78.5m, "Negeri"),
    ];

    /// <summary>
    /// Trims, de-duplicates and validates an incoming school ID selection.
    /// Shared by every school lookup path so validation rules and messages stay identical.
    /// </summary>
    public static bool TryNormalizeIds(
        IReadOnlyList<string>? requestedIds,
        out string[] ids,
        out string? error)
    {
        ids = requestedIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? [];

        if (ids.Length is < 2 or > 3)
        {
            ids = [];
            error = "Pilih tepat 2 atau 3 sekolah.";
            return false;
        }

        error = null;
        return true;
    }

    /// <summary>
    /// Builds a catalog record and derives the demo administrative fields so the
    /// seeded dataset exercises the ERD columns without hand-maintaining them.
    /// </summary>
    private static SchoolRecord S(
        int number,
        string name,
        string city,
        string district,
        string level,
        decimal literacy,
        decimal numeracy,
        string status)
    {
        var students = 180 + (number * 37 % 420);

        return new SchoolRecord(
            Id: $"DKI-{number:00}",
            Name: name,
            City: city,
            District: district,
            Level: level,
            LiteracyScore: literacy,
            NumeracyScore: numeracy,
            Npsn: $"20{number:00}{(number * 7) % 10000:0000}",
            Status: status,
            JumlahMurid: students,
            JumlahGuru: Math.Max(8, students / 16),
            RuangKelas: Math.Max(6, students / 28),
            TanggalData: new DateOnly(2026, 1, 15));
    }
}

public sealed record SchoolRecord(
    string Id,
    string Name,
    string City,
    string District,
    string Level,
    decimal LiteracyScore,
    decimal NumeracyScore,
    string Npsn,
    string Status,
    int JumlahMurid,
    int JumlahGuru,
    int RuangKelas,
    DateOnly TanggalData)
{
    public decimal CompositeScore => decimal.Round((LiteracyScore + NumeracyScore) / 2, 1);
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
