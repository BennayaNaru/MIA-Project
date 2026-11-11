using OpenSch.Domain.Entities;
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
        builder.Property(i => i.NamaFile).HasColumnName("nama_file").HasMaxLength(255).IsRequired();
        builder.Property(i => i.TanggalImpor).HasColumnName("tanggal_impor").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(i => i.JumlahBaris).HasColumnName("jumlah_baris");
        builder.Property(i => i.Sukses).HasColumnName("sukses");
        builder.Property(i => i.Pesan).HasColumnName("pesan");
    }
}
