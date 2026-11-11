using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenSch.Application.Common.Interfaces;
using OpenSch.Application.Services;
using OpenSch.Domain.Constants;
using OpenSch.Web.Infrastructure;

namespace OpenSch.Web.Endpoints;

/// <summary>
/// Administrator-only operations. Kept on their own route prefix so public school
/// reads can never be affected by an import failure.
/// </summary>
public sealed class AdminEndpoints : IEndpointGroup
{
    private const long MaxCsvBytes = 2 * 1024 * 1024;

    public static string RoutePrefix => "/api/admin";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetStatus, "me").RequireAuthorization();

        groupBuilder.MapGet(GetImports, "imports")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator));

        groupBuilder.MapPost(UploadCsv, "imports")
            .RequireAuthorization(policy => policy.RequireRole(Roles.Administrator))
            .DisableAntiforgery();
    }

    [EndpointSummary("Get the current user's administrator status")]
    public static Ok<AdminStatus> GetStatus(IUser user) =>
        TypedResults.Ok(new AdminStatus(user.Roles?.Contains(Roles.Administrator) ?? false));

    [EndpointSummary("List recent CSV imports")]
    [EndpointDescription("Returns the most recent import runs and their row-level outcomes.")]
    public static async Task<Ok<IReadOnlyList<ImportHistoryItem>>> GetImports(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var imports = await context.ImporData
            .AsNoTracking()
            .OrderByDescending(import => import.TanggalImpor)
            .Take(50)
            .Select(import => new ImportHistoryItem(
                import.Id,
                import.NamaFile,
                import.StatusImpor,
                import.JumlahBaris,
                import.JumlahBerhasil,
                import.JumlahGagal,
                import.TanggalImpor,
                import.Pesan))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok<IReadOnlyList<ImportHistoryItem>>(imports);
    }

    [EndpointSummary("Import schools from a CSV file")]
    [EndpointDescription("Validates the whole file, then upserts every row by NPSN in a single write. Invalid files change nothing and return a row-level error report.")]
    public static async Task<IResult> UploadCsv(
        IFormFile file,
        ISchoolImportService importer,
        IUser user,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "File kosong",
                Detail = "Pilih file CSV yang berisi data sekolah.",
            });
        }

        if (file.Length > MaxCsvBytes)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "File terlalu besar",
                Detail = "Ukuran maksimum file CSV adalah 2 MB.",
            });
        }

        if (!string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Format file salah",
                Detail = "Hanya file dengan ekstensi .csv yang didukung.",
            });
        }

        await using var stream = file.OpenReadStream();
        var report = await importer.ImportAsync(stream, file.FileName, user.Id, cancellationToken);

        return report.Succeeded
            ? TypedResults.Ok(report)
            : TypedResults.BadRequest(report);
    }
}

public sealed record AdminStatus(bool IsAdministrator);

public sealed record ImportHistoryItem(
    long Id,
    string FileName,
    string Status,
    int TotalRows,
    int Succeeded,
    int Failed,
    DateTimeOffset ImportedAt,
    string? Message);
