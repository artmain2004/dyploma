using CatalogService.Data;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Middleware;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class AdminCatalogService : IAdminCatalogService
{
    private readonly CatalogDbContext _db;
    private readonly IBlobStorageService _blobStorage;

    public AdminCatalogService(CatalogDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }

    public async Task<List<AdminCategoryDto>> GetCategoriesAsync()
    {
        return await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AdminCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<AdminCategoryDto> CreateCategoryAsync(AdminCategoryCreateRequest request)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new AdminCategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<AdminCategoryDto> UpdateCategoryAsync(Guid id, AdminCategoryUpdateRequest request)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            throw new ApiException(404, "Category not found");
        }

        category.Name = request.Name;
        await _db.SaveChangesAsync();

        return new AdminCategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            throw new ApiException(404, "Category not found");
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AdminProductDto>> GetProductsAsync(string? search, Guid? categoryId)
    {
        var query = _db.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var terms = search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            foreach (var term in terms)
            {
                var pattern = $"%{term}%";
                query = query.Where(p =>
                    EF.Functions.ILike(p.Name, pattern) ||
                    (p.Description != null && EF.Functions.ILike(p.Description, pattern)) ||
                    (p.Category != null && EF.Functions.ILike(p.Category.Name, pattern)));
            }
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        return await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new AdminProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsFeatured = p.IsFeatured,
                CategoryId = p.CategoryId
            })
            .ToListAsync();
    }

    public async Task<AdminProductDto> CreateProductAsync(AdminProductCreateRequest request)
    {
        string? imageUrl = request.ImageUrl;
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            imageUrl = await _blobStorage.UploadAsync(request.ImageFile);
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
            IsFeatured = request.IsFeatured,
            CreatedAtUtc = DateTime.UtcNow,
            CategoryId = request.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return new AdminProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId
        };
    }

    public async Task<AdminProductDto> UpdateProductAsync(Guid id, AdminProductUpdateRequest request)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            throw new ApiException(404, "Product not found");
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.IsFeatured = request.IsFeatured;
        product.CategoryId = request.CategoryId;

        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            product.ImageUrl = await _blobStorage.UploadAsync(request.ImageFile);
        }
        else if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            product.ImageUrl = request.ImageUrl;
        }

        await _db.SaveChangesAsync();

        return new AdminProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            IsFeatured = product.IsFeatured,
            CategoryId = product.CategoryId
        };
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            throw new ApiException(404, "Product not found");
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}
