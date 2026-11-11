using System.Reflection;
using OpenSch.Application.Common.Interfaces;
using OpenSch.Domain.Entities;
using OpenSch.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OpenSch.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Sekolah> Sekolah => Set<Sekolah>();
    public DbSet<Perbandingan> Perbandingan => Set<Perbandingan>();
    public DbSet<DetailPerbandingan> DetailPerbandingan => Set<DetailPerbandingan>();
    public DbSet<ImporData> ImporData => Set<ImporData>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
