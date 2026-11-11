namespace OpenSch.Application.Services;

/// <summary>
/// Summary metrics for a deterministic comparison. This type is shared by the
/// HTTP endpoints and the unit tests so both use exactly the same calculation.
/// </summary>
public sealed record ComparisonMetrics(
    string LiteracyLeaderId,
    string NumeracyLeaderId,
    string CompositeLeaderId,
    decimal LiteracyRange,
    decimal NumeracyRange,
    bool IsFairComparisonGroup);

public static class MetricsCalculator
{
    public static decimal GetSafeLiteracyScore(decimal? score) => score ?? 0m;

    public static decimal GetSafeNumeracyScore(decimal? score) => score ?? 0m;

    public static decimal GetSafeCompositeScore(decimal? literacy, decimal? numeracy) =>
        decimal.Round((GetSafeLiteracyScore(literacy) + GetSafeNumeracyScore(numeracy)) / 2m, 1);

    /// <summary>
    /// Computes the leaders, score ranges and fair-comparison flag used by both
    /// comparison endpoints. Callers must pass between 2 and 3 resolved schools.
    /// </summary>
    public static ComparisonMetrics Calculate(IReadOnlyList<SchoolDto> schools)
    {
        ArgumentNullException.ThrowIfNull(schools);

        if (schools.Count == 0)
        {
            throw new ArgumentException("At least one school is required to calculate comparison metrics.", nameof(schools));
        }

        var literacyScores = schools.Select(s => GetSafeLiteracyScore(s.LiteracyScore)).ToArray();
        var numeracyScores = schools.Select(s => GetSafeNumeracyScore(s.NumeracyScore)).ToArray();

        var literacyLeader = schools.MaxBy(s => GetSafeLiteracyScore(s.LiteracyScore))!;
        var numeracyLeader = schools.MaxBy(s => GetSafeNumeracyScore(s.NumeracyScore))!;
        var compositeLeader = schools.MaxBy(s => GetSafeCompositeScore(s.LiteracyScore, s.NumeracyScore))!;

        return new ComparisonMetrics(
            literacyLeader.Id,
            numeracyLeader.Id,
            compositeLeader.Id,
            decimal.Round(literacyScores.Max() - literacyScores.Min(), 1),
            decimal.Round(numeracyScores.Max() - numeracyScores.Min(), 1),
            schools.Select(s => (s.Level, s.City)).Distinct().Count() == 1);
    }
}
