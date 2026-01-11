using OrderService.Entities;

namespace OrderService.DTOs;

public class OrderCreateResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string? PromoCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
