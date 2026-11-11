namespace OpenSch.Web.Services;

/// <summary>
/// Cloudflare Turnstile configuration. The site key is public and is exposed to the
/// browser through <c>GET /api/config/public</c>; the secret key must never leave the server.
/// </summary>
public sealed class TurnstileOptions
{
    public const string SectionName = "Turnstile";

    public bool Enabled { get; set; }

    public string SiteKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string VerifyUrl { get; set; } = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
}
