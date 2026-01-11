using OrderService.Entities;

namespace OrderService.DTOs;

public class AdminOrderListResponse
{
    public List<OrderSummaryResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}

public class AdminOrderUpdateStatusRequest
{
    public OrderStatus Status { get; set; }
}
