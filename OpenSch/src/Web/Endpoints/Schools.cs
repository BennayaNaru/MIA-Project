using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenSch.Application.Services;
using OpenSch.Web.Infrastructure;

namespace OpenSch.Web.Endpoints;

public sealed class Schools : IEndpointGroup
{
    public static string RoutePrefix => "/api/schools";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetSchools);
        groupBuilder.MapGet(GetRankingSummary, "ranking/summary");
    }

    [EndpointSummary("List schools")]
    [EndpointDescription("Returns the schools stored in the database. Each rank is scoped to its level and city group.")]
    public static async Task<Ok<IReadOnlyList<SchoolDto>>> GetSchools(
        ISchoolQueryService schoolQuery,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await schoolQuery.GetAllAsync(cancellationToken));

    [EndpointSummary("Get a ranking summary for one level and city")]
    [EndpointDescription("Returns the top schools for a single education level and city, plus a deterministic rule-based summary. Ranks are scoped to that group.")]
    public static async Task<Results<Ok<RankingSummary>, BadRequest<ValidationProblemDetails>>> GetRankingSummary(
        [FromQuery] string level,
        [FromQuery] string city,
        [FromQuery] int? limit,
        ISchoolQueryService schoolQuery,
        CancellationToken cancellationToken)
    {
        var result = await schoolQuery.GetRankingAsync(level, city, limit ?? 10, cancellationToken);

        if (!result.Succeeded)
        {
            return TypedResults.BadRequest(new ValidationProblemDetails(
                new Dictionary<string, string[]>
                {
                    [result.Field ?? "scope"] = [result.Error!],
                }));
        }

        return TypedResults.Ok(result.Summary!);
    }
}
