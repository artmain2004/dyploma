namespace OrderService.Entities;

public class PromoCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public PromoCodeType Type { get; set; }
    public decimal Value { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public int? UsageLimit { get; set; }
    public int TimesUsed { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
