using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Middleware;

namespace OrderService.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly OrderDbContext _db;

    public PromoCodeService(OrderDbContext db)
    {
        _db = db;
    }

    public async Task<PromoCodeResult> ValidateAsync(ValidatePromoCodeRequest request)
    {
        var code = request.Code.Trim();
        var promo = await GetValidPromoAsync(code);
        var discount = CalculateDiscount(promo, request.Total);
        var totalAfter = Math.Max(0, request.Total - discount);

        return new PromoCodeResult
        {
            Code = promo.Code,
            Type = promo.Type,
            Value = promo.Value,
            DiscountAmount = discount,
            TotalAfterDiscount = totalAfter
        };
    }

    public async Task<(string? promoCode, decimal discount)> ApplyAsync(string? code, decimal total)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return (null, 0m);
        }

        var promo = await GetValidPromoAsync(code.Trim());
        var discount = CalculateDiscount(promo, total);
        promo.TimesUsed += 1;
        await _db.SaveChangesAsync();

        return (promo.Code, discount);
    }

    private async Task<PromoCode> GetValidPromoAsync(string code)
    {
        var promo = await _db.PromoCodes.FirstOrDefaultAsync(p => p.Code == code);
        if (promo == null)
        {
            throw new ApiException(404, "Promo code not found");
        }

        if (!promo.IsActive)
        {
            throw new ApiException(400, "Promo code is inactive");
        }

        if (promo.ExpiresAtUtc.HasValue && promo.ExpiresAtUtc.Value < DateTime.UtcNow)
        {
            throw new ApiException(400, "Promo code expired");
        }

        if (promo.UsageLimit.HasValue && promo.TimesUsed >= promo.UsageLimit.Value)
        {
            throw new ApiException(400, "Promo code usage limit reached");
        }

        return promo;
    }

    private static decimal CalculateDiscount(PromoCode promo, decimal total)
    {
        if (total <= 0)
        {
            return 0m;
        }

        return promo.Type switch
        {
            PromoCodeType.Fixed => Math.Min(total, promo.Value),
            PromoCodeType.Percent => Math.Min(total, total * (promo.Value / 100m)),
            _ => 0m
        };
    }
}
