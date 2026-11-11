using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using OpenSch.Web.Infrastructure;
using OpenSch.Web.Services;

namespace OpenSch.Web.Endpoints;

/// <summary>
/// Configuration that is safe to expose to the browser. Secrets never appear here.
/// </summary>
public sealed class PublicConfig : IEndpointGroup
{
    public static string RoutePrefix => "/api/config";

    public static void Map(RouteGroupBuilder groupBuilder) =>
        groupBuilder.MapGet(GetPublicConfig, "public");

    [EndpointSummary("Get public client configuration")]
    [EndpointDescription("Returns runtime settings the browser needs, such as the Turnstile site key.")]
    public static Ok<PublicClientConfig> GetPublicConfig(IOptions<TurnstileOptions> options)
    {
        var turnstile = options.Value;

        return TypedResults.Ok(new PublicClientConfig(
            TurnstileEnabled: turnstile.Enabled,
            TurnstileSiteKey: turnstile.Enabled ? turnstile.SiteKey : string.Empty));
    }
}

public sealed record PublicClientConfig(bool TurnstileEnabled, string TurnstileSiteKey);
