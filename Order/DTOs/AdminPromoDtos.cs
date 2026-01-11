using System.ComponentModel.DataAnnotations;
using OrderService.Entities;

namespace OrderService.DTOs;

public class PromoCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public PromoCodeType Type { get; set; }
    public decimal Value { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public int? UsageLimit { get; set; }
    public int TimesUsed { get; set; }
}

public class PromoCodeCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    public PromoCodeType Type { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Value { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public int? UsageLimit { get; set; }
}

public class PromoCodeUpdateRequest
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    public PromoCodeType Type { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Value { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public int? UsageLimit { get; set; }
}
