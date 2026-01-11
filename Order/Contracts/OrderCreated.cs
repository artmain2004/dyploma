namespace Contracts;

public record OrderCreated
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = "";
    public string CustomerEmail { get; init; } = "";
    public string? CustomerName { get; init; }
    public decimal TotalPrice { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public List<OrderCreatedItem> Items { get; init; } = new();
}

public record OrderCreatedItem
{
    public string ProductName { get; init; } = "";
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}
