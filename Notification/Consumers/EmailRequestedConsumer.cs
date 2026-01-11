using Contracts;
using MassTransit;
using NotificationService.Services;
using NotificationService.Templates;

namespace NotificationService.Consumers;

public class EmailRequestedConsumer : IConsumer<EmailRequested>
{
    private readonly IEmailSender _emailSender;

    public EmailRequestedConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Consume(ConsumeContext<EmailRequested> context)
    {
        var message = context.Message;
        var body = message.Template switch
        {
            EmailTemplateType.ConfirmEmail => EmailTemplates.ConfirmEmailTemplate(message.FirstName, message.ActionUrl ?? ""),
            EmailTemplateType.ResetPassword => EmailTemplates.ResetPasswordTemplate(message.FirstName, message.ActionUrl ?? ""),
            _ => string.Empty
        };
        if (string.IsNullOrWhiteSpace(body))
        {
            return;
        }
        await _emailSender.SendAsync(message.ToEmail, message.Subject, body);
    }
}
