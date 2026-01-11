using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Middleware;
using OrderService.Services;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<OrderCreateResponse>> Create([FromBody] OrderCreateRequest request)
    {
        var userId = GetUserIdOrNull();
        var response = await _orderService.CreateAsync(request, userId);
        return Ok(response);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<List<OrderSummaryResponse>>> GetMyOrders()
    {
        var userId = GetRequiredUserId();
        var orders = await _orderService.GetForUserAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<OrderDetailsResponse>> GetById(Guid id)
    {
        var userId = GetRequiredUserId();
        var order = await _orderService.GetByIdAsync(userId, id);
        return Ok(order);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult<OrderDetailsResponse>> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var userId = GetRequiredUserId();
        var order = await _orderService.UpdateStatusAsync(userId, id, request);
        return Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize]
    public async Task<ActionResult<OrderDetailsResponse>> Cancel(Guid id)
    {
        var userId = GetRequiredUserId();
        var order = await _orderService.CancelAsync(userId, id);
        return Ok(order);
    }

    private Guid? GetUserIdOrNull()
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private Guid GetRequiredUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (Guid.TryParse(value, out var id))
        {
            return id;
        }

        throw new ApiException(401, "Unauthorized");
    }
}
