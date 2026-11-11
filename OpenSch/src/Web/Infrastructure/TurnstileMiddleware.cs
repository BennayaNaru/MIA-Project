using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenSch.Web.Services;

namespace OpenSch.Web.Infrastructure;

/// <summary>
/// Requires a valid Cloudflare Turnstile token for registration. Login is left alone so
/// existing demo accounts keep working even if the Turnstile script is blocked.
/// </summary>
public sealed class TurnstileMiddleware(
    RequestDelegate next,
    IOptions<TurnstileOptions> options,
    TurnstileVerifier verifier)
{
    private const string TokenHeader = "X-Turnstile-Token";
    private static readonly PathString RegisterPath = new("/api/Users/register");

    public async Task InvokeAsync(HttpContext context)
    {
        var config = options.Value;

        if (!config.Enabled || !IsProtectedRequest(context.Request))
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers[TokenHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(token))
        {
            await RejectAsync(context, "Token verifikasi tidak ditemukan.");
            return;
        }

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        if (!await verifier.VerifyAsync(token, remoteIp, context.RequestAborted))
        {
            await RejectAsync(context, "Verifikasi Turnstile tidak valid atau sudah kedaluwarsa.");
            return;
        }

        await next(context);
    }

    private static bool IsProtectedRequest(HttpRequest request) =>
        HttpMethods.IsPost(request.Method) &&
        request.Path.StartsWithSegments(RegisterPath, StringComparison.OrdinalIgnoreCase);

    private static async Task RejectAsync(HttpContext context, string detail)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Verifikasi keamanan gagal",
            Detail = detail,
        });
    }
}
