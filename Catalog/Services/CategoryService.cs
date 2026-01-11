using CatalogService.Data;
using CatalogService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class CategoryService : ICategoryService
{
    private readonly CatalogDbContext _db;

    public CategoryService(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }
}
