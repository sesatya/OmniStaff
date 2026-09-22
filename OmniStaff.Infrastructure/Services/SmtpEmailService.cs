using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using OmniStaff.Application.Interfaces;

namespace OmniStaff.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(host)) return;

        var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 25;
        var from = _config["Smtp:From"] ?? "noreply@example.com";
        var user = _config["Smtp:Username"];
        var pass = _config["Smtp:Password"];

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true
        };

        if (!string.IsNullOrWhiteSpace(user))
        {
            client.Credentials = new NetworkCredential(user, pass);
        }

        var msg = new MailMessage(from, to, subject, body) { IsBodyHtml = false };
        await client.SendMailAsync(msg);
    }
}
