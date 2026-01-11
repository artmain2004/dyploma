using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponse>> Get()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest request)
    {
        var userId = GetUserId();
        var cart = await _cartService.AddItemAsync(userId, request);
        return Ok(cart);
    }

    [HttpPatch("items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItem(Guid productId, [FromBody] UpdateCartItemRequest request)
    {
        var userId = GetUserId();
        var cart = await _cartService.UpdateItemAsync(userId, productId, request);
        return Ok(cart);
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid productId)
    {
        var userId = GetUserId();
        var cart = await _cartService.RemoveItemAsync(userId, productId);
        return Ok(cart);
    }

    [HttpDelete]
    public async Task<ActionResult<CartResponse>> Clear()
    {
        var userId = GetUserId();
        var cart = await _cartService.ClearAsync(userId);
        return Ok(cart);
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out var userId))
        {
            throw new OrderService.Middleware.ApiException(401, "Unauthorized");
        }
        return userId;
    }
}
