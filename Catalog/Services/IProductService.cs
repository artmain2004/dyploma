using CatalogService.DTOs;

namespace CatalogService.Services;

public interface IProductService
{
    Task<ProductListResponse> GetProductsAsync(ProductQuery query);
    Task<ProductDetailsDto?> GetByIdAsync(Guid id);
}
