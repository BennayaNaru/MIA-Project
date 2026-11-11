namespace OpenSch.Domain.Entities;

public class Perbandingan
{
    public long Id { get; set; }
    public string SekolahSumberId { get; set; } = string.Empty;
    public string SekolahTujuanId { get; set; } = string.Empty;
    public DateTimeOffset TanggalPerbandingan { get; set; }
    public string MetodePerbandingan { get; set; } = string.Empty;
    public string? PemimpinHurufId { get; set; }
    public string? PemimpinAngkaId { get; set; }
    public string? PemimpinKompositId { get; set; }
    public decimal? RentangHuruf { get; set; }
    public decimal? RentangAngka { get; set; }
    public bool? PerbandinganAdil { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Sekolah SekolahSumber { get; set; } = null!;
    public Sekolah SekolahTujuan { get; set; } = null!;
    public ICollection<DetailPerbandingan> Details { get; } = new List<DetailPerbandingan>();
}
