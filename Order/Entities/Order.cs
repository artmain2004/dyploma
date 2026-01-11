namespace OrderService.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public Guid? UserId { get; set; }
    public string CustomerEmail { get; set; } = null!;
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ShippingAddress { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public string? PromoCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}
