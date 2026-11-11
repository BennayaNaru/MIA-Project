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
    [EndpointDescription("Uses persisted school data to produce a deterministic comparison. Provide 2 or 3 schoolIds.")]
    public static async Task<Results<Ok<DeterministicComparison>, BadRequest<ValidationProblemDetails>>> CreateDeterministicComparison(
        CompareSchoolsRequest request,
        ISchoolQueryService schoolQuery,
        CancellationToken cancellationToken)
    {
        var selection = await schoolQuery.ResolveForComparisonAsync(request.SchoolIds, cancellationToken);
        if (!selection.Succeeded)
        {
            return TypedResults.BadRequest(ToValidationProblem(selection.Error!));
        }

        return TypedResults.Ok(new DeterministicComparison(
            selection.Schools,
            MetricsCalculator.Calculate(selection.Schools)));
    }

    [EndpointSummary("Create an AI school comparison")]
    [EndpointDescription("Sends a standardized JSON payload built from persisted school data to the configured AI provider. The response always includes an AI accuracy warning.")]
    public static async Task<Results<Ok<AiComparisonResponse>, BadRequest<ValidationProblemDetails>>> CreateAiComparison(
        CompareSchoolsRequest request,
        ISchoolQueryService schoolQuery,
        IAiComparisonService comparisonService,
        CancellationToken cancellationToken)
    {
        var selection = await schoolQuery.ResolveForComparisonAsync(request.SchoolIds, cancellationToken);
        if (!selection.Succeeded)
        {
            return TypedResults.BadRequest(ToValidationProblem(selection.Error!));
        }

        var result = await comparisonService.CompareAsync(selection.Schools, cancellationToken);
        return TypedResults.Ok(new AiComparisonResponse(
            result.Source,
            selection.Schools,
            MetricsCalculator.Calculate(selection.Schools),
            result.Narrative,
            result.Warnings,
            result.AnalysisPayload));
    }

    private static ValidationProblemDetails ToValidationProblem(string message) =>
        new(new Dictionary<string, string[]> { ["schoolIds"] = [message] });
}

public sealed record CompareSchoolsRequest(IReadOnlyList<string>? SchoolIds);
public sealed record DeterministicComparison(IReadOnlyList<SchoolDto> Schools, ComparisonMetrics Metrics);
public sealed record AiComparisonResponse(string Source, IReadOnlyList<SchoolDto> Schools, ComparisonMetrics Metrics, AiNarrative Narrative, IReadOnlyList<string> Warnings, AiComparisonPayload AnalysisPayload);
