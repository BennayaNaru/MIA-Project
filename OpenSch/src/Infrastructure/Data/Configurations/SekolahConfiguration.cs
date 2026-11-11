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
        builder.Property(s => s.Nama).HasColumnName("nama").HasMaxLength(255).IsRequired();
        builder.Property(s => s.Kota).HasColumnName("kota").HasMaxLength(100).IsRequired();
        builder.Property(s => s.KabupatenKota).HasColumnName("kabupaten_kota").HasMaxLength(100).IsRequired();
        builder.Property(s => s.Level).HasColumnName("level").HasMaxLength(20).IsRequired();
        builder.Property(s => s.SkorMelekHuruf).HasColumnName("skor_melek_huruf").HasPrecision(5, 2);
        builder.Property(s => s.SkorMelekAngka).HasColumnName("skor_melek_angka").HasPrecision(5, 2);
        builder.Property(s => s.SkorKomposit).HasColumnName("skor_komposit").HasPrecision(5, 2);
        builder.Property(s => s.Peringkat).HasColumnName("peringkat");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
