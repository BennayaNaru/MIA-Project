using OpenSch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSch.Infrastructure.Data.Configurations;

public class SekolahConfiguration : IEntityTypeConfiguration<Sekolah>
{
    public void Configure(EntityTypeBuilder<Sekolah> builder)
    {
        builder.ToTable("sekolah");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasColumnName("id").HasMaxLength(20);
        builder.Property(s => s.Npsn).HasColumnName("npsn").HasMaxLength(20);
        builder.Property(s => s.Nama).HasColumnName("nama").HasMaxLength(255).IsRequired();
        builder.Property(s => s.Kota).HasColumnName("kota").HasMaxLength(100).IsRequired();
        builder.Property(s => s.KabupatenKota).HasColumnName("kabupaten_kota").HasMaxLength(100).IsRequired();
        builder.Property(s => s.Level).HasColumnName("level").HasMaxLength(20).IsRequired();
        builder.Property(s => s.Status).HasColumnName("status").HasMaxLength(20);
        builder.Property(s => s.JumlahMurid).HasColumnName("jumlah_murid");
        builder.Property(s => s.JumlahGuru).HasColumnName("jumlah_guru");
        builder.Property(s => s.RuangKelas).HasColumnName("ruang_kelas");
        builder.Property(s => s.SkorMelekHuruf).HasColumnName("skor_melek_huruf").HasPrecision(5, 2);
        builder.Property(s => s.SkorMelekAngka).HasColumnName("skor_melek_angka").HasPrecision(5, 2);
        builder.Property(s => s.SkorKomposit).HasColumnName("skor_komposit").HasPrecision(5, 2);
        builder.Property(s => s.Peringkat).HasColumnName("peringkat");
        builder.Property(s => s.TanggalData).HasColumnName("tanggal_data");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        // NPSN is the natural key used to upsert CSV imports. Multiple NULLs are allowed
        // so legacy/synthetic rows without an NPSN remain valid.
        builder.HasIndex(s => s.Npsn).IsUnique();
    }
}
