using Identity.Data;
using Identity.Models;
using Identity.Requets;
using Identity.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Controllers;

[ApiController]
[Route("api/profile/addresses")]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IdentityDbContext _db;

    public AddressesController(IdentityDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetAll()
    {
        var userId = GetUserId();
        var items = await _db.UserAddresses.AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAtUtc)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                Label = a.Label,
                Line1 = a.Line1,
                Line2 = a.Line2,
                City = a.City,
                Region = a.Region,
                PostalCode = a.PostalCode,
                Country = a.Country,
                Phone = a.Phone,
                IsDefault = a.IsDefault,
                CreatedAtUtc = a.CreatedAtUtc
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create([FromBody] AddressCreateRequest request)
    {
        var userId = GetUserId();
        if (request.IsDefault)
        {
            await ClearDefaultAsync(userId);
        }

        var address = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Label = request.Label,
            Line1 = request.Line1,
            Line2 = request.Line2,
            City = request.City,
            Region = request.Region,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Phone = request.Phone,
            IsDefault = request.IsDefault,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.UserAddresses.Add(address);
        await _db.SaveChangesAsync();

        return Ok(new AddressDto
        {
            Id = address.Id,
            Label = address.Label,
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            Region = address.Region,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Phone = address.Phone,
            IsDefault = address.IsDefault,
            CreatedAtUtc = address.CreatedAtUtc
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> Update(Guid id, [FromBody] AddressUpdateRequest request)
    {
        var userId = GetUserId();
        var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address == null)
        {
            return NotFound();
        }

        if (request.IsDefault && !address.IsDefault)
        {
            await ClearDefaultAsync(userId);
        }

        address.Label = request.Label;
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.City = request.City;
        address.Region = request.Region;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country;
        address.Phone = request.Phone;
        address.IsDefault = request.IsDefault;

        await _db.SaveChangesAsync();

        return Ok(new AddressDto
        {
            Id = address.Id,
            Label = address.Label,
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            Region = address.Region,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Phone = address.Phone,
            IsDefault = address.IsDefault,
            CreatedAtUtc = address.CreatedAtUtc
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var address = await _db.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address == null)
        {
            return NotFound();
        }

        _db.UserAddresses.Remove(address);
        await _db.SaveChangesAsync();
        return Ok();
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException();
        }
        return userId;
    }

    private async Task ClearDefaultAsync(string userId)
    {
        var currentDefaults = await _db.UserAddresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
        foreach (var address in currentDefaults)
        {
            address.IsDefault = false;
        }
    }
}
