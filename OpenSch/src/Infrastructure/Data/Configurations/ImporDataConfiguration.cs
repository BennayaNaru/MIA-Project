using OpenSch.Domain.Entities;
using OpenSch.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenSch.Infrastructure.Data.Configurations;

public class ImporDataConfiguration : IEntityTypeConfiguration<ImporData>
{
    public void Configure(EntityTypeBuilder<ImporData> builder)
    {
        builder.ToTable("impor_data");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(i => i.PenggunaId).HasColumnName("pengguna_id").HasMaxLength(450);
        builder.Property(i => i.NamaFile).HasColumnName("nama_file").HasMaxLength(255).IsRequired();
        builder.Property(i => i.StatusImpor).HasColumnName("status_impor").HasMaxLength(20).IsRequired();
        builder.Property(i => i.TanggalImpor).HasColumnName("tanggal_impor").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(i => i.JumlahBaris).HasColumnName("jumlah_baris");
        builder.Property(i => i.JumlahBerhasil).HasColumnName("jumlah_berhasil");
        builder.Property(i => i.JumlahGagal).HasColumnName("jumlah_gagal");
        builder.Property(i => i.Sukses).HasColumnName("sukses");
        builder.Property(i => i.Pesan).HasColumnName("pesan");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(i => i.PenggunaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
