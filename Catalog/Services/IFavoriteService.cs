using CatalogService.DTOs;

namespace CatalogService.Services;

public interface IFavoriteService
{
    Task<List<ProductListItemDto>> GetFavoritesAsync(string userId);
    Task AddFavoriteAsync(Guid productId, string userId);
    Task RemoveFavoriteAsync(Guid productId, string userId);
}
