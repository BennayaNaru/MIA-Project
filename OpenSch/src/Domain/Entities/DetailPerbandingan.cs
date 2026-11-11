namespace OpenSch.Domain.Entities;

public class DetailPerbandingan
{
    public long Id { get; set; }
    public long PerbandinganId { get; set; }
    public string SekolahId { get; set; } = string.Empty;
    public string Metrik { get; set; } = string.Empty;
    public decimal? NilaiAsli { get; set; }
    public decimal? NilaiNormalisasi { get; set; }
    public int? Rangking { get; set; }

    public Perbandingan Perbandingan { get; set; } = null!;
    public Sekolah Sekolah { get; set; } = null!;
}
