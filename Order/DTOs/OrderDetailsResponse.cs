using OrderService.Entities;

namespace OrderService.DTOs;

public class OrderDetailsResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string? PromoCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public string? ShippingAddress { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
