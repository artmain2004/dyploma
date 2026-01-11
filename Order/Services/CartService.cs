using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Middleware;

namespace OrderService.Services;

public class CartService : ICartService
{
    private readonly OrderDbContext _db;

    public CartService(OrderDbContext db)
    {
        _db = db;
    }

    public async Task<CartResponse> GetAsync(Guid userId)
    {
        return await LoadAsync(userId);
    }

    public async Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request)
    {
        var cartId = await GetOrCreateCartIdAsync(userId);

        var updated = await _db.CartItems
            .Where(i => i.CartId == cartId && i.ProductId == request.ProductId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(i => i.Quantity, i => i.Quantity + request.Quantity)
                .SetProperty(i => i.UnitPrice, request.UnitPrice)
                .SetProperty(i => i.ProductName, request.ProductName)
                .SetProperty(i => i.ImageUrl, request.ImageUrl));

        if (updated == 0)
        {
            await _db.CartItems.AddAsync(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice,
                Quantity = request.Quantity,
                ImageUrl = request.ImageUrl
            });
            await _db.SaveChangesAsync();
        }

        return await LoadAsync(userId);
    }

    public async Task<CartResponse> UpdateItemAsync(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        var cartId = await GetOrCreateCartIdAsync(userId);

        var updated = await _db.CartItems
            .Where(i => i.CartId == cartId && i.ProductId == productId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(i => i.Quantity, request.Quantity));

        if (updated == 0)
        {
            throw new ApiException(404, "Cart item not found");
        }

        return await LoadAsync(userId);
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, Guid productId)
    {
        var cartId = await GetOrCreateCartIdAsync(userId);

        await _db.CartItems
            .Where(i => i.CartId == cartId && i.ProductId == productId)
            .ExecuteDeleteAsync();

        return await LoadAsync(userId);
    }

    public async Task<CartResponse> ClearAsync(Guid userId)
    {
        var cartId = await GetOrCreateCartIdAsync(userId);
        await _db.CartItems.Where(i => i.CartId == cartId).ExecuteDeleteAsync();
        return await LoadAsync(userId);
    }

    private async Task<CartResponse> LoadAsync(Guid userId)
    {
        var cart = await _db.Carts.AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        return Map(cart);
    }

    private async Task<Guid> GetOrCreateCartIdAsync(Guid userId)
    {
        var cartId = await _db.Carts.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (cartId != Guid.Empty)
        {
            return cartId;
        }

        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Carts.Add(cart);
        try
        {
            await _db.SaveChangesAsync();
            return cart.Id;
        }
        catch (DbUpdateException)
        {
            var existingId = await _db.Carts.AsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
            if (existingId != Guid.Empty)
            {
                return existingId;
            }
            throw;
        }
    }

    private static CartResponse Map(Cart? cart)
    {
        if (cart == null)
        {
            return new CartResponse();
        }

        return new CartResponse
        {
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
            }).ToList()
        };
    }
}
