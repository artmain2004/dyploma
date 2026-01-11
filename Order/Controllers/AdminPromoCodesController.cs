using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Middleware;

namespace OrderService.Controllers;

[ApiController]
[Route("api/admin/promocodes")]
[Authorize(Roles = "Admin")]
public class AdminPromoCodesController : ControllerBase
{
    private readonly OrderDbContext _db;

    public AdminPromoCodesController(OrderDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PromoCodeDto>>> GetAll()
    {
        var items = await _db.PromoCodes.AsNoTracking()
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => new PromoCodeDto
            {
                Id = p.Id,
                Code = p.Code,
                Type = p.Type,
                Value = p.Value,
                IsActive = p.IsActive,
                ExpiresAtUtc = p.ExpiresAtUtc,
                UsageLimit = p.UsageLimit,
                TimesUsed = p.TimesUsed
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<PromoCodeDto>> Create([FromBody] PromoCodeCreateRequest request)
    {
        var exists = await _db.PromoCodes.AnyAsync(p => p.Code == request.Code);
        if (exists)
        {
            throw new ApiException(400, "Promo code already exists");
        }

        var promo = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = request.Code.Trim(),
            Type = request.Type,
            Value = request.Value,
            IsActive = request.IsActive,
            ExpiresAtUtc = request.ExpiresAtUtc.HasValue ? DateTime.SpecifyKind(request.ExpiresAtUtc.Value, DateTimeKind.Utc) : null,
            UsageLimit = request.UsageLimit,
            TimesUsed = 0,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.PromoCodes.Add(promo);
        await _db.SaveChangesAsync();

        return Ok(new PromoCodeDto
        {
            Id = promo.Id,
            Code = promo.Code,
            Type = promo.Type,
            Value = promo.Value,
            IsActive = promo.IsActive,
            ExpiresAtUtc = promo.ExpiresAtUtc,
            UsageLimit = promo.UsageLimit,
            TimesUsed = promo.TimesUsed
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PromoCodeDto>> Update(Guid id, [FromBody] PromoCodeUpdateRequest request)
    {
        var promo = await _db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
        if (promo == null)
        {
            throw new ApiException(404, "Promo code not found");
        }

        promo.Code = request.Code.Trim();
        promo.Type = request.Type;
        promo.Value = request.Value;
        promo.IsActive = request.IsActive;
        promo.ExpiresAtUtc = request.ExpiresAtUtc.HasValue ? DateTime.SpecifyKind(request.ExpiresAtUtc.Value, DateTimeKind.Utc) : null;
        promo.UsageLimit = request.UsageLimit;

        await _db.SaveChangesAsync();

        return Ok(new PromoCodeDto
        {
            Id = promo.Id,
            Code = promo.Code,
            Type = promo.Type,
            Value = promo.Value,
            IsActive = promo.IsActive,
            ExpiresAtUtc = promo.ExpiresAtUtc,
            UsageLimit = promo.UsageLimit,
            TimesUsed = promo.TimesUsed
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var promo = await _db.PromoCodes.FirstOrDefaultAsync(p => p.Id == id);
        if (promo == null)
        {
            throw new ApiException(404, "Promo code not found");
        }

        _db.PromoCodes.Remove(promo);
        await _db.SaveChangesAsync();

        return Ok();
    }
}
