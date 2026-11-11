using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace OpenSch.Web.Services;

/// <summary>
/// Verifies a Turnstile token with Cloudflare. Verification is intentionally server-side:
/// a browser can render the widget but cannot decide whether the challenge passed.
/// </summary>
public sealed class TurnstileVerifier(
    HttpClient httpClient,
    IOptions<TurnstileOptions> options,
    ILogger<TurnstileVerifier> logger)
{
    public async Task<bool> VerifyAsync(string token, string? remoteIp, CancellationToken cancellationToken)
    {
        var config = options.Value;

        if (!config.Enabled)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(config.SecretKey))
        {
            logger.LogError("Turnstile is enabled but Turnstile:SecretKey is empty.");
            return false;
        }

        var form = new Dictionary<string, string>
        {
            ["secret"] = config.SecretKey,
            ["response"] = token,
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            form["remoteip"] = remoteIp;
        }

        try
        {
            using var response = await httpClient.PostAsync(
                config.VerifyUrl,
                new FormUrlEncodedContent(form),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Turnstile verification returned HTTP {StatusCode}.",
                    (int)response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<TurnstileResponse>(cancellationToken);

            if (result?.Success == true)
            {
                return true;
            }

            logger.LogWarning(
                "Turnstile rejected a token. Errors: {Errors}",
                result?.ErrorCodes is { Length: > 0 } errors ? string.Join(", ", errors) : "unknown");

            return false;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(exception, "Turnstile verification could not be completed.");
            return false;
        }
    }

    private sealed record TurnstileResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("error-codes")] string[]? ErrorCodes);
}
