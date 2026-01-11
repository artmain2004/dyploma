using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(OrderDbContext db)
    {
        if (await db.PromoCodes.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        var promos = new List<PromoCode>
        {
            new PromoCode
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Code = "WELCOME10",
                Type = PromoCodeType.Percent,
                Value = 10m,
                IsActive = true,
                ExpiresAtUtc = now.AddMonths(3),
                UsageLimit = null,
                TimesUsed = 0,
                CreatedAtUtc = now
            },
            new PromoCode
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Code = "SAVE5",
                Type = PromoCodeType.Fixed,
                Value = 5m,
                IsActive = true,
                ExpiresAtUtc = now.AddMonths(1),
                UsageLimit = 500,
                TimesUsed = 0,
                CreatedAtUtc = now
            }
        };

        await db.PromoCodes.AddRangeAsync(promos);
        await db.SaveChangesAsync();
    }
}
