using CatalogService.DTOs;

namespace CatalogService.Services;

public interface IAdminCatalogService
{
    Task<List<AdminCategoryDto>> GetCategoriesAsync();
    Task<AdminCategoryDto> CreateCategoryAsync(AdminCategoryCreateRequest request);
    Task<AdminCategoryDto> UpdateCategoryAsync(Guid id, AdminCategoryUpdateRequest request);
    Task DeleteCategoryAsync(Guid id);

    Task<List<AdminProductDto>> GetProductsAsync(string? search, Guid? categoryId);
    Task<AdminProductDto> CreateProductAsync(AdminProductCreateRequest request);
    Task<AdminProductDto> UpdateProductAsync(Guid id, AdminProductUpdateRequest request);
    Task DeleteProductAsync(Guid id);
}
