using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs;

public class AddCartItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = "";

    [Range(0, 1000000)]
    public decimal UnitPrice { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }
}
