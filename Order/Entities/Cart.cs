namespace OrderService.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<CartItem> Items { get; set; } = new();
}
