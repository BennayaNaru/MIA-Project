using Microsoft.AspNetCore.Http.HttpResults;
using OpenSch.Application.Services;
using OpenSch.Web.Infrastructure;

namespace OpenSch.Web.Endpoints;

public sealed class Schools : IEndpointGroup
{
    public static string RoutePrefix => "/api/schools";

    public static void Map(RouteGroupBuilder groupBuilder) => groupBuilder.MapGet(GetSchools);

    [EndpointSummary("List schools")]
    [EndpointDescription("Returns the schools stored in the database, ranked by composite then literacy score.")]
    public static async Task<Ok<IReadOnlyList<SchoolDto>>> GetSchools(
        ISchoolQueryService schoolQuery,
        CancellationToken cancellationToken)
        => TypedResults.Ok(await schoolQuery.GetAllAsync(cancellationToken));
}
