using CatalogService.Data;
using CatalogService.Entities;
using Testcontainers.PostgreSql;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catalog.Tests;

public class CatalogIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("catalog_test")
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
    public async Task CanPersistProductInPostgres()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        await using var db = new CatalogDbContext(options);
        await db.Database.MigrateAsync();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Seed",
            Price = 9.99m,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        var count = await db.Products.CountAsync();
        count.Should().Be(1);
    }
}
