using System.ComponentModel.DataAnnotations;
using OrderService.Entities;

namespace OrderService.DTOs;

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}
