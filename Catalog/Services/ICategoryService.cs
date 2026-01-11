using CatalogService.DTOs;

namespace CatalogService.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
}
