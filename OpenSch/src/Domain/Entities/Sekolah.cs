namespace OpenSch.Domain.Entities;

public class Sekolah
{
    public string Id { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Kota { get; set; } = string.Empty;
    public string KabupatenKota { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal? SkorMelekHuruf { get; set; }
    public decimal? SkorMelekAngka { get; set; }
    public decimal? SkorKomposit { get; set; }
    public int? Peringkat { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
