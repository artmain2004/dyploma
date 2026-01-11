namespace OrderService.DTOs;

public class CartResponse
{
    public List<CartItemDto> Items { get; set; } = new();
}
