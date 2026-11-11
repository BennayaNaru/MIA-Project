using OpenSch.Application.Services;
using OpenSch.Domain.Constants;
using OpenSch.Domain.Entities;
using OpenSch.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
        _configuration = configuration;
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
                await EnsurePenggunaViewAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    /// <summary>
    /// SQLite uses EnsureCreated instead of migrations, so the read-only ERD-aligned
    /// <c>pengguna</c> view has to be created explicitly. PostgreSQL gets it from the
    /// AddPenggunaView migration.
    /// </summary>
    private async Task EnsurePenggunaViewAsync()
    {
        if (!_context.Database.IsSqlite())
        {
            return;
        }

        await _context.Database.ExecuteSqlRawAsync("DROP VIEW IF EXISTS pengguna;");
        await _context.Database.ExecuteSqlRawAsync(
            """
            CREATE VIEW pengguna AS
            SELECT
                u."Id" AS id,
                u."UserName" AS nama,
                u."Email" AS email,
                COALESCE(
                    (
                        SELECT MIN(r."Name")
                        FROM "AspNetUserRoles" ur
                        JOIN "AspNetRoles" r ON r."Id" = ur."RoleId"
                        WHERE ur."UserId" = u."Id"
                    ),
                    'Pengguna'
                ) AS peran,
                u."EmailConfirmed" AS email_terkonfirmasi
            FROM "AspNetUsers" u;
            """);
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

        await TrySeedAdministratorAsync();
        await TrySeedSchoolsAsync();
    }

    /// <summary>
    /// Creates the first administrator from configuration secrets. Nothing is seeded
    /// unless <c>AdminBootstrap:Email</c> and <c>AdminBootstrap:Password</c> are set, so
    /// no known password ever lives in source control.
    /// </summary>
    private async Task TrySeedAdministratorAsync()
    {
        var email = _configuration["AdminBootstrap:Email"];
        var password = _configuration["AdminBootstrap:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var created = await _userManager.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                throw new InvalidOperationException(
                    "Unable to create the bootstrap administrator: " +
                    string.Join("; ", created.Errors.Select(error => error.Description)));
            }

            _logger.LogInformation("Bootstrap administrator {Email} created.", email);
        }

        if (!await _userManager.IsInRoleAsync(user, Roles.Administrator))
        {
            await _userManager.AddToRoleAsync(user, Roles.Administrator);
        }
    }

    /// <summary>
    /// Seeds the demo catalogue. Existing rows are only backfilled for columns added
    /// after the first seed, so an administrator's imported/edited values are never
    /// overwritten by a restart.
    /// </summary>
    private async Task TrySeedSchoolsAsync()
    {
        var existing = await _context.Sekolah
            .ToDictionaryAsync(school => school.Id, StringComparer.Ordinal);

        var seededAt = DateTimeOffset.UtcNow;

        foreach (var record in SchoolCatalog.All)
        {
            if (existing.TryGetValue(record.Id, out var school))
            {
                school.Npsn ??= record.Npsn;
                school.Status ??= record.Status;
                school.JumlahMurid ??= record.JumlahMurid;
                school.JumlahGuru ??= record.JumlahGuru;
                school.RuangKelas ??= record.RuangKelas;
                school.TanggalData ??= record.TanggalData;
                continue;
            }

            _context.Sekolah.Add(new Sekolah
            {
                Id = record.Id,
                Npsn = record.Npsn,
                Nama = record.Name,
                Kota = record.City,
                KabupatenKota = record.District,
                Level = record.Level,
                Status = record.Status,
                JumlahMurid = record.JumlahMurid,
                JumlahGuru = record.JumlahGuru,
                RuangKelas = record.RuangKelas,
                SkorMelekHuruf = record.LiteracyScore,
                SkorMelekAngka = record.NumeracyScore,
                SkorKomposit = record.CompositeScore,
                TanggalData = record.TanggalData,
                CreatedAt = seededAt,
                UpdatedAt = seededAt,
            });
        }

        await _context.SaveChangesAsync();
    }
}
