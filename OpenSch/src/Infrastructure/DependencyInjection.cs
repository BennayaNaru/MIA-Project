using OpenSch.Application.Common.Interfaces;
using OpenSch.Infrastructure.Data;
using OpenSch.Infrastructure.Data.Interceptors;
using OpenSch.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var isDevelopment = builder.Environment.IsDevelopment() ||
                            builder.Environment.IsEnvironment("Testing");

        var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider")?.Trim();
        if (string.IsNullOrWhiteSpace(databaseProvider))
        {
            throw new InvalidOperationException(
                "Required configuration key 'DatabaseProvider' is missing. " +
                "Set it to 'Postgres' (production/Aspire) or 'Sqlite' (local development).");
        }

        var isSqlite = databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase);
        var allowSqliteOutsideDevelopment =
            builder.Configuration.GetValue<bool>("Database:AllowSqliteInProduction");

        // Never allow a SQLite deployment to slip into production by accident. Developers
        // who intentionally want it can opt in explicitly through configuration.
        if (isSqlite && !isDevelopment && !allowSqliteOutsideDevelopment)
        {
            throw new InvalidOperationException(
                "SQLite is disabled outside Development. Configure 'DatabaseProvider' as 'Postgres' " +
                "or explicitly opt in with 'Database:AllowSqliteInProduction=true'.");
        }

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // Resolve the connection string lazily so design-time tooling (for example
            // OpenAPI document generation) can build the host without a live database.
            var connectionString = builder.Configuration.GetConnectionString(Services.Database);
            Guard.Against.NullOrWhiteSpace(connectionString, message: $"Connection string '{Services.Database}' not found.");

            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            switch (databaseProvider.ToLowerInvariant())
            {
                case "postgres":
                    options.UseNpgsql(connectionString, postgres => postgres.EnableRetryOnFailure());
                    break;
                case "sqlite":
                    options.UseSqlite(connectionString);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported database provider '{databaseProvider}'. Use 'Postgres' or 'Sqlite'.");
            }

            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        // Password-reset emails require a real IEmailSender implementation. Use SMTP when
        // it is configured and fall back to logging in local development. Production
        // registrations without SMTP throw when used instead of silently discarding mail.
        builder.Services.Configure<EmailOptions>(
            builder.Configuration.GetSection(EmailOptions.SectionName));

        var smtpHost = builder.Configuration[$"{EmailOptions.SectionName}:Smtp:Host"];
        var fromAddress = builder.Configuration[$"{EmailOptions.SectionName}:FromAddress"];

        if (!string.IsNullOrWhiteSpace(smtpHost))
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException(
                    "Email:Smtp:Host is configured but Email:FromAddress is missing.");
            }

            builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
        }
        else if (isDevelopment)
        {
            builder.Services.AddTransient<IEmailSender, LoggingEmailSender>();
        }
        else
        {
            // Fail loudly when the feature is actually used instead of pretending that
            // the message was sent. Production deployments should always configure SMTP.
            builder.Services.AddTransient<IEmailSender, UnconfiguredEmailSender>();
        }
    }
}

/// <summary>
/// Development-only email sender that writes reset links to the application log instead
/// of sending real emails. Replace with a real IEmailSender before going to production.
/// </summary>
file sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogWarning(
            "[DEV ONLY] Email not sent — no real IEmailSender is configured. " +
            "Recipient: {Email} | Subject: {Subject} | Body: {Body}",
            email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}
