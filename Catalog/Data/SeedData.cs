using CatalogService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(CatalogDbContext db)
    {
        if (await db.Categories.AnyAsync() || await db.Products.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var categories = new List<Category>
        {
            new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Electronics" },
            new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Books" },
            new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Home" }
        };

        var products = new List<Product>
        {
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Wireless Headphones", Description = "Noise cancelling over-ear", Price = 199.99m, ImageUrl = "https://example.com/p1.jpg", IsFeatured = true, CreatedAtUtc = now.AddDays(-10), CategoryId = categories[0].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Smartphone", Description = "128GB storage", Price = 699.00m, ImageUrl = "https://example.com/p2.jpg", IsFeatured = true, CreatedAtUtc = now.AddDays(-9), CategoryId = categories[0].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Bluetooth Speaker", Description = "Portable speaker", Price = 79.50m, ImageUrl = "https://example.com/p3.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-8), CategoryId = categories[0].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "E-Reader", Description = "6-inch display", Price = 129.99m, ImageUrl = "https://example.com/p4.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-7), CategoryId = categories[0].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Sci-Fi Novel", Description = "Paperback edition", Price = 14.95m, ImageUrl = "https://example.com/p5.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-6), CategoryId = categories[1].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "Cookbook", Description = "Healthy recipes", Price = 24.00m, ImageUrl = "https://example.com/p6.jpg", IsFeatured = true, CreatedAtUtc = now.AddDays(-5), CategoryId = categories[1].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "Mystery Novel", Description = "Hardcover edition", Price = 19.99m, ImageUrl = "https://example.com/p7.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-4), CategoryId = categories[1].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "Desk Lamp", Description = "LED lamp", Price = 39.00m, ImageUrl = "https://example.com/p8.jpg", IsFeatured = true, CreatedAtUtc = now.AddDays(-3), CategoryId = categories[2].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), Name = "Coffee Maker", Description = "Drip coffee maker", Price = 89.00m, ImageUrl = "https://example.com/p9.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-2), CategoryId = categories[2].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), Name = "Vacuum Cleaner", Description = "Bagless vacuum", Price = 159.00m, ImageUrl = "https://example.com/p10.jpg", IsFeatured = false, CreatedAtUtc = now.AddDays(-1), CategoryId = categories[2].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000011"), Name = "Throw Pillow", Description = "Decorative pillow", Price = 18.50m, ImageUrl = "https://example.com/p11.jpg", IsFeatured = false, CreatedAtUtc = now, CategoryId = categories[2].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000012"), Name = "Notebook", Description = "Lined notebook", Price = 6.75m, ImageUrl = "https://example.com/p12.jpg", IsFeatured = false, CreatedAtUtc = now, CategoryId = categories[1].Id },
            new Product { Id = Guid.Parse("10000000-0000-0000-0000-000000000013"), Name = "USB-C Charger", Description = "Fast charging", Price = 29.99m, ImageUrl = "https://example.com/p13.jpg", IsFeatured = true, CreatedAtUtc = now, CategoryId = categories[0].Id }
        };

        await db.Categories.AddRangeAsync(categories);
        await db.Products.AddRangeAsync(products);
        await db.SaveChangesAsync();
    }
}
