using Microsoft.Extensions.Options;
using NotificationService.Settings;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Services;

public class EmailSender : IEmailSender
{
    private readonly MailSettings _settings;

    public EmailSender(IOptions<MailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
        };

        var from = new MailAddress(_settings.FromEmail, _settings.FromName);
        var to = new MailAddress(toEmail);
        using var message = new MailMessage(from, to)
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}
