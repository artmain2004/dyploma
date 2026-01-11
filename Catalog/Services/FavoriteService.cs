using CatalogService.Data;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Middleware;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class FavoriteService : IFavoriteService
{
    private readonly CatalogDbContext _db;

    public FavoriteService(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductListItemDto>> GetFavoritesAsync(string userId)
    {
        return await _db.Favorites.AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAtUtc)
            .Include(f => f.Product)
            .ThenInclude(p => p.Category)
            .Include(f => f.Product)
            .ThenInclude(p => p.Reviews)
            .Select(f => new ProductListItemDto
            {
                Id = f.Product.Id,
                Name = f.Product.Name,
                Description = f.Product.Description,
                Price = f.Product.Price,
                ImageUrl = f.Product.ImageUrl,
                IsFeatured = f.Product.IsFeatured,
                CreatedAtUtc = f.Product.CreatedAtUtc,
                CategoryId = f.Product.CategoryId,
                CategoryName = f.Product.Category != null ? f.Product.Category.Name : null,
                AverageRating = f.Product.Reviews.Count == 0 ? 0 : Math.Round(f.Product.Reviews.Average(r => r.Rating), 1),
                ReviewsCount = f.Product.Reviews.Count
            })
            .ToListAsync();
    }

    public async Task AddFavoriteAsync(Guid productId, string userId)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
        {
            throw new ApiException(404, "Product not found");
        }

        var exists = await _db.Favorites.AnyAsync(f => f.ProductId == productId && f.UserId == userId);
        if (exists)
        {
            return;
        }

        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Favorites.Add(favorite);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveFavoriteAsync(Guid productId, string userId)
    {
        var favorite = await _db.Favorites.FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId);
        if (favorite == null)
        {
            return;
        }

        _db.Favorites.Remove(favorite);
        await _db.SaveChangesAsync();
    }
}
