using Microsoft.AspNetCore.Http.HttpResults;
using OpenSch.Application.Services;
using OpenSch.Web.Infrastructure;

namespace OpenSch.Web.Endpoints;

public sealed class Schools : IEndpointGroup
{
    public static string RoutePrefix => "/api/schools";

    public static void Map(RouteGroupBuilder groupBuilder) => groupBuilder.MapGet(GetSchools);

    [EndpointSummary("List synthetic schools")]
    [EndpointDescription("Returns the canonical synthetic school metrics used by comparison endpoints.")]
    public static Ok<IReadOnlyList<SchoolDto>> GetSchools() => TypedResults.Ok(SchoolCatalog.GetAll());
}
