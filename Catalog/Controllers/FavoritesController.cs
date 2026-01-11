using CatalogService.DTOs;
using CatalogService.Middleware;
using CatalogService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductListItemDto>>> GetFavorites()
    {
        var userId = GetUserId();
        var favorites = await _favoriteService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }

    [HttpPost("{productId:guid}")]
    public async Task<ActionResult> AddFavorite(Guid productId)
    {
        var userId = GetUserId();
        await _favoriteService.AddFavoriteAsync(productId, userId);
        return Ok();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<ActionResult> RemoveFavorite(Guid productId)
    {
        var userId = GetUserId();
        await _favoriteService.RemoveFavoriteAsync(productId, userId);
        return Ok();
    }

    private string GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ApiException(401, "Unauthorized");
        }

        return userId;
    }
}
