using System.ComponentModel.DataAnnotations;
using OrderService.Entities;

namespace OrderService.DTOs;

public class ValidatePromoCodeRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal Total { get; set; }
}

public class PromoCodeResult
{
    public string Code { get; set; } = null!;
    public PromoCodeType Type { get; set; }
    public decimal Value { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAfterDiscount { get; set; }
}
