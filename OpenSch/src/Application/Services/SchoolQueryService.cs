using OpenSch.Application.Common.Interfaces;
using OpenSch.Domain.Entities;

namespace OpenSch.Application.Services;

/// <summary>
/// Reads school data from the database. <see cref="SchoolCatalog"/> is only used to
/// seed the database; every API read goes through this service so the persisted data
/// remains the single source of truth.
/// </summary>
public interface ISchoolQueryService
{
    /// <summary>Returns every persisted school, ranked by composite then literacy score.</summary>
    Task<IReadOnlyList<SchoolDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a user-supplied list of 2 or 3 school IDs against the database.
    /// IDs are trimmed and de-duplicated, and the original request order is preserved.
    /// </summary>
    Task<SchoolSelection> ResolveForComparisonAsync(
        IReadOnlyList<string>? requestedIds,
        CancellationToken cancellationToken = default);
}

public sealed record SchoolSelection(IReadOnlyList<SchoolDto> Schools, string? Error)
{
    public bool Succeeded => Error is null;
}

public sealed class SchoolQueryService(IApplicationDbContext context) : ISchoolQueryService
{
    public async Task<IReadOnlyList<SchoolDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var schools = await context.Sekolah
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Rank(schools);
    }

    public async Task<SchoolSelection> ResolveForComparisonAsync(
        IReadOnlyList<string>? requestedIds,
        CancellationToken cancellationToken = default)
    {
        if (!SchoolCatalog.TryNormalizeIds(requestedIds, out var ids, out var error))
        {
            return new SchoolSelection([], error);
        }

        var ranked = await GetAllAsync(cancellationToken);
        var byId = ranked.ToDictionary(school => school.Id, StringComparer.Ordinal);

        var missing = ids.Where(id => !byId.ContainsKey(id)).ToArray();
        if (missing.Length > 0)
        {
            return new SchoolSelection([], $"ID sekolah tidak ditemukan: {string.Join(", ", missing)}.");
        }

        return new SchoolSelection(ids.Select(id => byId[id]).ToList(), null);
    }

    private static IReadOnlyList<SchoolDto> Rank(IEnumerable<Sekolah> schools) =>
        schools
            .Select(ToDto)
            .OrderByDescending(school => school.CompositeScore)
            .ThenByDescending(school => school.LiteracyScore)
            .ThenBy(school => school.Id, StringComparer.Ordinal)
            .Select((school, index) => school with { Rank = index + 1 })
            .ToList();

    private static SchoolDto ToDto(Sekolah school)
    {
        var literacy = school.SkorMelekHuruf ?? 0m;
        var numeracy = school.SkorMelekAngka ?? 0m;
        var composite = school.SkorKomposit ?? MetricsCalculator.GetSafeCompositeScore(literacy, numeracy);

        return new SchoolDto(
            school.Id,
            school.Nama,
            school.Kota,
            school.KabupatenKota,
            school.Level,
            literacy,
            numeracy,
            composite,
            Rank: 0);
    }
}
