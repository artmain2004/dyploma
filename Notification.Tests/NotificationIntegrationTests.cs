using Contracts;
using Testcontainers.RabbitMq;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Consumers;
using NotificationService.Services;
using Xunit;

namespace Notification.Tests;

public class NotificationIntegrationTests : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbit = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();

    public async Task InitializeAsync()
    {
        await _rabbit.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbit.DisposeAsync();
    }

    [Fact]
    public async Task EmailRequested_IsConsumed()
    {
        var services = new ServiceCollection();
        var sender = new FakeEmailSender();
        services.AddSingleton<IEmailSender>(sender);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailRequestedConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(_rabbit.Hostname, _rabbit.GetMappedPublicPort(5672), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        await using var provider = services.BuildServiceProvider();
        var busControl = provider.GetRequiredService<IBusControl>();
        await busControl.StartAsync();
        try
        {
            var publish = provider.GetRequiredService<IPublishEndpoint>();
            await publish.Publish(new EmailRequested
            {
                ToEmail = "test@local",
                Subject = "Test",
                Template = EmailTemplateType.ConfirmEmail,
                FirstName = "Jan",
                ActionUrl = "https://example.com"
            });

            var result = await sender.WaitAsync(TimeSpan.FromSeconds(10));
            result.Should().BeTrue();
        }
        finally
        {
            await busControl.StopAsync();
        }
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        private readonly TaskCompletionSource<bool> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            _tcs.TrySetResult(true);
            return Task.CompletedTask;
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            return Task.WhenAny(_tcs.Task, Task.Delay(timeout)).ContinueWith(t => _tcs.Task.IsCompleted);
        }
    }
}
