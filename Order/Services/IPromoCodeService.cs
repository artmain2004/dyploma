using OrderService.DTOs;

namespace OrderService.Services;

public interface IPromoCodeService
{
    Task<PromoCodeResult> ValidateAsync(ValidatePromoCodeRequest request);
    Task<(string? promoCode, decimal discount)> ApplyAsync(string? code, decimal total);
}
