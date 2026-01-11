using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs;

public class OrderCreateRequest
{
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = null!;

    [MaxLength(200)]
    public string? CustomerName { get; set; }

    [MaxLength(40)]
    public string? CustomerPhone { get; set; }

    [MaxLength(400)]
    public string? ShippingAddress { get; set; }

    [Required]
    [MinLength(1)]
    public List<OrderCreateItemRequest> Items { get; set; } = new();

    [MaxLength(50)]
    public string? PromoCode { get; set; }
}

public class OrderCreateItemRequest
{
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
