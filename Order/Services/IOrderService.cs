using OrderService.DTOs;

namespace OrderService.Services;

public interface IOrderService
{
    Task<OrderCreateResponse> CreateAsync(OrderCreateRequest request, Guid? userId);
    Task<List<OrderSummaryResponse>> GetForUserAsync(Guid userId);
    Task<OrderDetailsResponse> GetByIdAsync(Guid userId, Guid orderId);
    Task<OrderDetailsResponse> UpdateStatusAsync(Guid userId, Guid orderId, UpdateOrderStatusRequest request);
    Task<OrderDetailsResponse> CancelAsync(Guid userId, Guid orderId);
}
