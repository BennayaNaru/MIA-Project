using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenSch.Application.Services;
using OpenSch.Web.Infrastructure;
using OpenSch.Web.Services;

namespace OpenSch.Web.Endpoints;

public sealed class Comparisons : IEndpointGroup
{
    public static string RoutePrefix => "/api/compare";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateDeterministicComparison);
        groupBuilder.MapPost(CreateAiComparison, "ai")
            .RequireAuthorization()
            .RequireRateLimiting("ai-per-user");
    }

    [EndpointSummary("Compare two or three schools")]
    [EndpointDescription("Uses canonical server data to produce a deterministic comparison. Provide 2 or 3 schoolIds.")]
    public static Results<Ok<DeterministicComparison>, BadRequest<ValidationProblemDetails>> CreateDeterministicComparison(CompareSchoolsRequest request)
    {
        if (!TryResolveSchools(request, out var schools, out var error))
        {
            return TypedResults.BadRequest(error!);
        }

        return TypedResults.Ok(new DeterministicComparison(schools, MetricsCalculator.Calculate(schools)));
    }

    [EndpointSummary("Create an AI school comparison")]
    [EndpointDescription("Sends a standardized JSON payload built from canonical server data to the configured AI provider. The response always includes an AI accuracy warning.")]
    public static async Task<Results<Ok<AiComparisonResponse>, BadRequest<ValidationProblemDetails>>> CreateAiComparison(
        CompareSchoolsRequest request,
        IAiComparisonService comparisonService,
        CancellationToken cancellationToken)
    {
        if (!TryResolveSchools(request, out var schools, out var error))
        {
            return TypedResults.BadRequest(error!);
        }

        var result = await comparisonService.CompareAsync(schools, cancellationToken);
        return TypedResults.Ok(new AiComparisonResponse(result.Source, schools, MetricsCalculator.Calculate(schools), result.Narrative, result.Warnings, result.AnalysisPayload));
    }

    private static bool TryResolveSchools(
        CompareSchoolsRequest request,
        out IReadOnlyList<SchoolDto> schools,
        out ValidationProblemDetails? error)
    {
        if (SchoolCatalog.TryResolve(request.SchoolIds, out schools, out var message))
        {
            error = null;
            return true;
        }

        error = new ValidationProblemDetails(new Dictionary<string, string[]> { ["schoolIds"] = [message!] });
        return false;
    }
}

public sealed record CompareSchoolsRequest(IReadOnlyList<string>? SchoolIds);
public sealed record DeterministicComparison(IReadOnlyList<SchoolDto> Schools, ComparisonMetrics Metrics);
public sealed record AiComparisonResponse(string Source, IReadOnlyList<SchoolDto> Schools, ComparisonMetrics Metrics, AiNarrative Narrative, IReadOnlyList<string> Warnings, AiComparisonPayload AnalysisPayload);
