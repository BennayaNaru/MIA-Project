namespace OpenSch.Application.Services;

/// <summary>
/// A ranking scoped to one education level and city. Ranks are only meaningful
/// inside that group, so every response names the scope explicitly.
/// </summary>
public sealed record RankingSummary(
    string Level,
    string City,
    int TotalSchools,
    IReadOnlyList<SchoolDto> Schools,
    RankingNarrative Narrative);

public sealed record RankingNarrative(
    string Headline,
    IReadOnlyList<string> Points,
    string ScopeNote);

public sealed record RankingResult(RankingSummary? Summary, string? Error, string? Field = null)
{
    public bool Succeeded => Error is null;

    public static RankingResult Success(RankingSummary summary) => new(summary, null);

    public static RankingResult Failure(string field, string error) => new(null, error, field);
}

/// <summary>
/// Builds the ranking narrative from the same numbers returned by the API. The
/// summary is deliberately rule-based so it cannot invent schools or outcomes.
/// </summary>
public static class RankingNarrativeFactory
{
    private const string ScopeNote =
        "Ringkasan ini dihitung langsung dari data peringkat (deterministik, tanpa AI). " +
        "Peringkat hanya berlaku dalam kelompok jenjang dan kota administrasi yang sama; " +
        "data sekolah bersifat sintetis untuk demo.";

    public static RankingNarrative Create(string level, string city, IReadOnlyList<SchoolDto> schools)
    {
        if (schools.Count == 0)
        {
            return new RankingNarrative(
                $"Belum ada data peringkat untuk {level} di {city}.",
                ["Pilih jenjang atau kota administrasi lain untuk melihat peringkat yang tersedia."],
                ScopeNote);
        }

        var leader = schools[0];
        var last = schools[^1];
        var average = decimal.Round(schools.Average(school => school.CompositeScore), 1);
        var spread = decimal.Round(leader.CompositeScore - last.CompositeScore, 1);

        var points = new List<string>
        {
            $"Peringkat 1 ditempati {leader.Name} ({leader.District}) dengan skor komposit " +
            $"{leader.CompositeScore}, literasi {leader.LiteracyScore}, dan numerasi {leader.NumeracyScore}.",
            $"Kelompok ini berisi {schools.Count} sekolah. Rata-rata skor komposit {average} dengan " +
            $"selisih {spread} poin antara peringkat 1 dan peringkat {schools.Count}.",
        };

        if (schools.Count >= 3)
        {
            points.Add(
                "Tiga teratas: " +
                string.Join("; ", schools.Take(3).Select(school =>
                    $"#{school.Rank} {school.Name} ({school.CompositeScore})")) +
                ".");
        }
        else if (schools.Count == 2)
        {
            points.Add($"Peringkat 2: {last.Name} ({last.CompositeScore}).");
        }

        return new RankingNarrative(
            $"{leader.Name} memimpin peringkat {level} di {city}.",
            points,
            ScopeNote);
    }
}
