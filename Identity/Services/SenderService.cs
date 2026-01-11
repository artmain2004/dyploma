using Identity.Options;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Identity.Services
{

    public interface ISenderService
    {
        Task SendMessageAsync(string to, string subject, string body);
    }
    public class SenderService : ISenderService
    {
        private readonly MailSettings _settings;

        public SenderService(IOptions<MailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendMessageAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();    
            message.From.Add(MailboxAddress.Parse(_settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.UserName,
                _settings.Password);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
