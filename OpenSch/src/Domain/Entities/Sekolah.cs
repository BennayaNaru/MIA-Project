namespace OpenSch.Domain.Entities;

public class Sekolah
{
    public string Id { get; set; } = string.Empty;

    /// <summary>Nomor Pokok Sekolah Nasional. Unique when present.</summary>
    public string? Npsn { get; set; }

    public string Nama { get; set; } = string.Empty;
    public string Kota { get; set; } = string.Empty;
    public string KabupatenKota { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;

    /// <summary>Negeri or Swasta.</summary>
    public string? Status { get; set; }

    public int? JumlahMurid { get; set; }
    public int? JumlahGuru { get; set; }
    public int? RuangKelas { get; set; }

    public decimal? SkorMelekHuruf { get; set; }
    public decimal? SkorMelekAngka { get; set; }
    public decimal? SkorKomposit { get; set; }
    public int? Peringkat { get; set; }

    /// <summary>Date the source data was measured.</summary>
    public DateOnly? TanggalData { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
