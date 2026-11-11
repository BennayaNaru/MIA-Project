namespace OpenSch.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<OpenSch.Domain.Entities.Sekolah> Sekolah { get; }
    DbSet<OpenSch.Domain.Entities.Perbandingan> Perbandingan { get; }
    DbSet<OpenSch.Domain.Entities.DetailPerbandingan> DetailPerbandingan { get; }
    DbSet<OpenSch.Domain.Entities.ImporData> ImporData { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
