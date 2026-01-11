using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Services;
using Xunit;

namespace Order.Tests;

public class PromoCodeServiceTests
{
    [Fact]
    public async Task ValidateAsync_AppliesPercentDiscount()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase($"order-{Guid.NewGuid()}")
            .Options;

        await using var db = new OrderDbContext(options);
        db.PromoCodes.Add(new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = "SAVE10",
            Type = PromoCodeType.Percent,
            Value = 10,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var service = new PromoCodeService(db);
        var result = await service.ValidateAsync(new ValidatePromoCodeRequest { Code = "SAVE10", Total = 200m });

        result.DiscountAmount.Should().Be(20m);
        result.TotalAfterDiscount.Should().Be(180m);
    }
}
