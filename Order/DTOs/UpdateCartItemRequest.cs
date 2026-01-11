using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs;

public class UpdateCartItemRequest
{
    [Range(1, 1000)]
    public int Quantity { get; set; }
}
