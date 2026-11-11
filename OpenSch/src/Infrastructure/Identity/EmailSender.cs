using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace OpenSch.Infrastructure.Identity;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string? FromAddress { get; init; }
    public string? FromName { get; init; }
    public SmtpOptions Smtp { get; init; } = new();
}

public sealed class SmtpOptions
{
    public string? Host { get; init; }
    public int Port { get; init; } = 587;
    public bool UseSsl { get; init; } = true;
    public string? UserName { get; init; }
    public string? Password { get; init; }
}

/// <summary>
/// Sends password-reset and confirmation emails through the SMTP server configured
/// under <c>Email:Smtp</c>. Production startup fails fast when this is not configured.
/// </summary>
public sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var config = options.Value;
        var smtp = config.Smtp;

        if (string.IsNullOrWhiteSpace(smtp.Host))
            throw new InvalidOperationException("Email:Smtp:Host is not configured.");

        if (string.IsNullOrWhiteSpace(config.FromAddress))
            throw new InvalidOperationException("Email:FromAddress is not configured.");

        using var message = new MailMessage
        {
            From = new MailAddress(config.FromAddress, config.FromName ?? config.FromAddress),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true,
        };
        message.To.Add(email);

        using var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(smtp.UserName)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(smtp.UserName, smtp.Password),
        };

        await client.SendMailAsync(message);
    }
}

/// <summary>
/// Used when no SMTP server is configured outside development. It deliberately throws
/// instead of silently discarding password-reset and confirmation emails.
/// </summary>
public sealed class UnconfiguredEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage) =>
        throw new InvalidOperationException(
            "No email sender is configured. Set Email:Smtp:Host and Email:FromAddress before " +
            "using password reset or email confirmation.");
}
