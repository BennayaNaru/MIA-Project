namespace OpenSch.Domain.Entities;

public class ImporData
{
    public long Id { get; set; }
    public string NamaFile { get; set; } = string.Empty;
    public DateTimeOffset TanggalImpor { get; set; }
    public int JumlahBaris { get; set; }
    public bool Sukses { get; set; }
    public string? Pesan { get; set; }
}
