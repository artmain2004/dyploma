using CatalogService.Data;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catalog.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductsAsync_LimitsPageSizeTo50()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase($"catalog-{Guid.NewGuid()}")
            .Options;

        await using var db = new CatalogDbContext(options);
        for (var i = 0; i < 60; i++)
        {
            db.Products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Item {i}",
                Price = 10 + i,
                CreatedAtUtc = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        var service = new ProductService(db);
        var response = await service.GetProductsAsync(new ProductQuery { Page = 1, PageSize = 100 });

        response.PageSize.Should().Be(50);
        response.Items.Should().HaveCount(50);
        response.TotalCount.Should().Be(60);
    }

    [Fact]
    public async Task GetProductsAsync_FiltersBySearch()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase($"catalog-{Guid.NewGuid()}")
            .Options;

        await using var db = new CatalogDbContext(options);
        db.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Alpha", Price = 10, CreatedAtUtc = DateTime.UtcNow });
        db.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Beta", Price = 12, CreatedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var service = new ProductService(db);
        var response = await service.GetProductsAsync(new ProductQuery { Search = "Al" });

        response.Items.Should().ContainSingle();
        response.Items[0].Name.Should().Be("Alpha");
    }
}
