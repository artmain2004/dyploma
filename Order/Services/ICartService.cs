using OrderService.DTOs;

namespace OrderService.Services;

public interface ICartService
{
    Task<CartResponse> GetAsync(Guid userId);
    Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request);
    Task<CartResponse> UpdateItemAsync(Guid userId, Guid productId, UpdateCartItemRequest request);
    Task<CartResponse> RemoveItemAsync(Guid userId, Guid productId);
    Task<CartResponse> ClearAsync(Guid userId);
}
