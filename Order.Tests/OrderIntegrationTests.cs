using Testcontainers.PostgreSql;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Entities;
using OrderService.Services;
using Xunit;

namespace Order.Tests;

public class OrderIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("order_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithImage("postgres:16")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    [Fact]
    public async Task ApplyAsync_IncrementsUsage()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        await using var db = new OrderDbContext(options);
        await db.Database.MigrateAsync();

        var promo = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = "ONCE",
            Type = PromoCodeType.Fixed,
            Value = 10,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.PromoCodes.Add(promo);
        await db.SaveChangesAsync();

        var service = new PromoCodeService(db);
        var result = await service.ApplyAsync("ONCE", 100m);

        result.discount.Should().Be(10m);
        var updated = await db.PromoCodes.FirstAsync(p => p.Code == "ONCE");
        updated.TimesUsed.Should().Be(1);
    }
}
