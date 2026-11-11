using OpenSch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSch.Infrastructure.Data.Configurations;

public class DetailPerbandinganConfiguration : IEntityTypeConfiguration<DetailPerbandingan>
{
    public void Configure(EntityTypeBuilder<DetailPerbandingan> builder)
    {
        builder.ToTable("detail_perbandingan");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(d => d.PerbandinganId).HasColumnName("perbandingan_id");
        builder.Property(d => d.SekolahId).HasColumnName("sekolah_id").HasMaxLength(20).IsRequired();
        builder.Property(d => d.Metrik).HasColumnName("metrik").HasMaxLength(50).IsRequired();
        builder.Property(d => d.NilaiAsli).HasColumnName("nilai_asli").HasPrecision(5, 2);
        builder.Property(d => d.NilaiNormalisasi).HasColumnName("nilai_normalisasi").HasPrecision(5, 2);
        builder.Property(d => d.Rangking).HasColumnName("rangking");

        builder.HasOne(d => d.Perbandingan)
            .WithMany(p => p.Details)
            .HasForeignKey(d => d.PerbandinganId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Sekolah)
            .WithMany()
            .HasForeignKey(d => d.SekolahId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
