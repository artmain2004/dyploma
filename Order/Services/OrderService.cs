using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Middleware;
using Contracts;
using MassTransit;

namespace OrderService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _db;
    private readonly IPromoCodeService _promoCodes;
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(OrderDbContext db, IPromoCodeService promoCodes, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _promoCodes = promoCodes;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderCreateResponse> CreateAsync(OrderCreateRequest request, Guid? userId)
    {
        var now = DateTime.UtcNow;
        var orderNumber = await GenerateOrderNumberAsync(now);

        var subtotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);
        var promoResult = await _promoCodes.ApplyAsync(request.PromoCode, subtotal);
        var totalPrice = Math.Max(0, subtotal - promoResult.discount);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            UserId = userId,
            CustomerEmail = request.CustomerEmail,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            ShippingAddress = request.ShippingAddress,
            Status = OrderStatus.New,
            CreatedAtUtc = now,
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };

        order.TotalPrice = totalPrice;
        order.PromoCode = promoResult.promoCode;
        order.DiscountAmount = promoResult.discount;

        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();

        await _publishEndpoint.Publish(new OrderCreated
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerEmail = order.CustomerEmail,
            CustomerName = order.CustomerName,
            TotalPrice = order.TotalPrice,
            CreatedAtUtc = order.CreatedAtUtc,
            Items = order.Items.Select(i => new OrderCreatedItem
            {
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        });

        return new OrderCreateResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            PromoCode = order.PromoCode,
            DiscountAmount = order.DiscountAmount,
            CreatedAtUtc = order.CreatedAtUtc
        };
    }

    public async Task<List<OrderSummaryResponse>> GetForUserAsync(Guid userId)
    {
        return await _db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderSummaryResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAtUtc = o.CreatedAtUtc
            })
            .ToListAsync();
    }

    public async Task<OrderDetailsResponse> GetByIdAsync(Guid userId, Guid orderId)
    {
        var order = await _db.Orders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new ApiException(404, "Order not found");
        }

        if (order.UserId != userId)
        {
            throw new ApiException(403, "Forbidden");
        }

        return MapDetails(order);
    }

    public async Task<OrderDetailsResponse> UpdateStatusAsync(Guid userId, Guid orderId, UpdateOrderStatusRequest request)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            throw new ApiException(404, "Order not found");
        }

        if (order.UserId != userId)
        {
            throw new ApiException(403, "Forbidden");
        }

        order.Status = request.Status;
        await _db.SaveChangesAsync();

        return MapDetails(order);
    }

    public async Task<OrderDetailsResponse> CancelAsync(Guid userId, Guid orderId)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            throw new ApiException(404, "Order not found");
        }

        if (order.UserId != userId)
        {
            throw new ApiException(403, "Forbidden");
        }

        if (order.Status != OrderStatus.New)
        {
            throw new ApiException(400, "Only new orders can be cancelled");
        }

        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync();

        return MapDetails(order);
    }

    private async Task<string> GenerateOrderNumberAsync(DateTime now)
    {
        var date = now.ToString("yyyyMMdd");
        for (var i = 0; i < 25; i++)
        {
            var suffix = Random.Shared.Next(0, 10000).ToString("D4");
            var number = $"ORD-{date}-{suffix}";
            var exists = await _db.Orders.AnyAsync(o => o.OrderNumber == number);
            if (!exists)
            {
                return number;
            }
        }

        throw new ApiException(500, "Could not generate order number");
    }

    private static OrderDetailsResponse MapDetails(Order order)
    {
        return new OrderDetailsResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            PromoCode = order.PromoCode,
            DiscountAmount = order.DiscountAmount,
            CreatedAtUtc = order.CreatedAtUtc,
            CustomerEmail = order.CustomerEmail,
            ShippingAddress = order.ShippingAddress,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
