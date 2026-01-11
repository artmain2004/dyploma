using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Middleware;
using OrderService.Services;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly IOrderService _orders;

    public AdminOrdersController(OrderDbContext db, IOrderService orders)
    {
        _db = db;
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<AdminOrderListResponse>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.Orders.AsNoTracking().OrderByDescending(o => o.CreatedAtUtc);
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderSummaryResponse
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAtUtc = o.CreatedAtUtc
            })
            .ToListAsync();

        return Ok(new AdminOrderListResponse
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailsResponse>> GetOrder(Guid id)
    {
        var order = await _db.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            throw new ApiException(404, "Order not found");
        }

        return Ok(new OrderDetailsResponse
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
        });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderDetailsResponse>> UpdateStatus(Guid id, [FromBody] AdminOrderUpdateStatusRequest request)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            throw new ApiException(404, "Order not found");
        }

        order.Status = request.Status;
        await _db.SaveChangesAsync();

        return Ok(new OrderDetailsResponse
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
        });
    }
}
