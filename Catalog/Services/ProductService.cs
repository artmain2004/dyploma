using CatalogService.Data;
using CatalogService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _db;

    public ProductService(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<ProductListResponse> GetProductsAsync(ProductQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 12 : query.PageSize;
        if (pageSize > 50)
        {
            pageSize = 50;
        }

        var products = _db.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Reviews).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var terms = query.Search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var term in terms)
            {
                var pattern = $"%{term}%";
                products = products.Where(p =>
                    EF.Functions.ILike(p.Name, pattern) ||
                    (p.Description != null && EF.Functions.ILike(p.Description, pattern)) ||
                    (p.Category != null && EF.Functions.ILike(p.Category.Name, pattern)));
            }
        }

        if (query.CategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == query.CategoryId);
        }

        if (query.Featured.HasValue)
        {
            products = products.Where(p => p.IsFeatured == query.Featured.Value);
        }

        if (query.MinPrice.HasValue)
        {
            products = products.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= query.MaxPrice.Value);
        }

        products = query.Sort?.ToLowerInvariant() switch
        {
            "price_asc" => products.OrderBy(p => p.Price).ThenByDescending(p => p.CreatedAtUtc),
            "price_desc" => products.OrderByDescending(p => p.Price).ThenByDescending(p => p.CreatedAtUtc),
            "newest" => products.OrderByDescending(p => p.CreatedAtUtc),
            "rating_desc" => products.OrderByDescending(p => p.Reviews.Count == 0 ? 0 : p.Reviews.Average(r => r.Rating)),
            _ => products.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await products.CountAsync();

        var items = await products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                CreatedAtUtc = p.CreatedAtUtc,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                AverageRating = p.Reviews.Count == 0 ? 0 : Math.Round(p.Reviews.Average(r => r.Rating), 1),
                ReviewsCount = p.Reviews.Count
            })
            .ToListAsync();

        return new ProductListResponse
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDetailsDto?> GetByIdAsync(Guid id)
    {
        return await _db.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Reviews)
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                CreatedAtUtc = p.CreatedAtUtc,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                AverageRating = p.Reviews.Count == 0 ? 0 : Math.Round(p.Reviews.Average(r => r.Rating), 1),
                ReviewsCount = p.Reviews.Count
            })
            .FirstOrDefaultAsync();
    }
}
