using FluentAssertions;
using NotificationService.Templates;
using Xunit;

namespace Notification.Tests;

public class EmailTemplatesTests
{
    [Fact]
    public void ConfirmEmailTemplate_ContainsPolishText()
    {
        var html = EmailTemplates.ConfirmEmailTemplate("Jan", "https://example.com");
        html.Should().Contain("Potwierdz email");
    }

    [Fact]
    public void ResetPasswordTemplate_ContainsPolishText()
    {
        var html = EmailTemplates.ResetPasswordTemplate("Jan", "https://example.com");
        html.Should().Contain("Reset hasla");
    }

    [Fact]
    public void OrderCreatedTemplate_ContainsOrderNumber()
    {
        var html = EmailTemplates.OrderCreatedTemplate("Jan", "ORD-20260101-0001", 10m, new List<string> { "Item" });
        html.Should().Contain("ORD-20260101-0001");
    }
}
