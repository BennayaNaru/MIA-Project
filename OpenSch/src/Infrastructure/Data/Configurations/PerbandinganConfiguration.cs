using OpenSch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSch.Infrastructure.Data.Configurations;

public class PerbandinganConfiguration : IEntityTypeConfiguration<Perbandingan>
{
    public void Configure(EntityTypeBuilder<Perbandingan> builder)
    {
        builder.ToTable("perbandingan");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(p => p.SekolahSumberId).HasColumnName("sekolah_sumber_id").HasMaxLength(20).IsRequired();
        builder.Property(p => p.SekolahTujuanId).HasColumnName("sekolah_tujuan_id").HasMaxLength(20).IsRequired();
        builder.Property(p => p.TanggalPerbandingan).HasColumnName("tanggal_perbandingan").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.MetodePerbandingan).HasColumnName("metode_perbandingan").HasMaxLength(50).IsRequired();
        builder.Property(p => p.PemimpinHurufId).HasColumnName("pemimpin_huruf_id").HasMaxLength(20);
        builder.Property(p => p.PemimpinAngkaId).HasColumnName("pemimpin_angka_id").HasMaxLength(20);
        builder.Property(p => p.PemimpinKompositId).HasColumnName("pemimpin_komposit_id").HasMaxLength(20);
        builder.Property(p => p.RentangHuruf).HasColumnName("rentang_huruf").HasPrecision(5, 2);
        builder.Property(p => p.RentangAngka).HasColumnName("rentang_angka").HasPrecision(5, 2);
        builder.Property(p => p.PerbandinganAdil).HasColumnName("perbandingan_adil");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(p => p.SekolahSumber)
            .WithMany()
            .HasForeignKey(p => p.SekolahSumberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.SekolahTujuan)
            .WithMany()
            .HasForeignKey(p => p.SekolahTujuanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
