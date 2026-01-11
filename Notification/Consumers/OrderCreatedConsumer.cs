using Contracts;
using MassTransit;
using NotificationService.Services;
using NotificationService.Templates;

namespace NotificationService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly IEmailSender _emailSender;

    public OrderCreatedConsumer(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;
        var subject = $"Order {message.OrderNumber}";
        var items = message.Items.Select(i => $"{i.ProductName} x{i.Quantity} - {i.UnitPrice:F2}").ToList();
        var body = EmailTemplates.OrderCreatedTemplate(message.CustomerName, message.OrderNumber, message.TotalPrice, items);
        await _emailSender.SendAsync(message.CustomerEmail, subject, body);
    }
}
