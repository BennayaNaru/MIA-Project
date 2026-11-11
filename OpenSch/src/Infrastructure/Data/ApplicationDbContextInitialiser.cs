using OpenSch.Application.Services;
using OpenSch.Domain.Constants;
using OpenSch.Domain.Entities;
using OpenSch.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenSch.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
            else
            {
                // Keep SQLite as a zero-setup local/test fallback.
                await _context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Administrative accounts must be created through an explicit, protected
        // provisioning process; never seed a known password into application code.

        if (!_context.Sekolah.Any())
        {
            var seededAt = DateTimeOffset.UtcNow;

            // Seed directly from the canonical catalog so persisted ranks can never
            // drift from the ranks returned by SchoolCatalog.GetAll().
            var catalog = SchoolCatalog.GetAll();

            _context.Sekolah.AddRange(catalog.Select(s => new Sekolah
            {
                Id             = s.Id,
                Nama           = s.Name,
                Kota           = s.City,
                KabupatenKota  = s.District,
                Level          = s.Level,
                SkorMelekHuruf = s.LiteracyScore,
                SkorMelekAngka = s.NumeracyScore,
                SkorKomposit   = s.CompositeScore,
                Peringkat      = s.Rank,
                CreatedAt      = seededAt,
                UpdatedAt      = seededAt,
            }));

            await _context.SaveChangesAsync();
        }
    }
}
