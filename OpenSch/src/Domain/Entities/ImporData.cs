namespace OpenSch.Domain.Entities;

public class ImporData
{
    public long Id { get; set; }

    /// <summary>Identity user that uploaded the file.</summary>
    public string? PenggunaId { get; set; }

    public string NamaFile { get; set; } = string.Empty;

    /// <summary>BERHASIL or GAGAL.</summary>
    public string StatusImpor { get; set; } = string.Empty;

    public DateTimeOffset TanggalImpor { get; set; }

    public int JumlahBaris { get; set; }
    public int JumlahBerhasil { get; set; }
    public int JumlahGagal { get; set; }

    /// <summary>Retained for backwards compatibility with the original two-state import log.</summary>
    public bool Sukses { get; set; }

    public string? Pesan { get; set; }
}
