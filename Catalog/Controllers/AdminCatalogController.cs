using CatalogService.DTOs;
using CatalogService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminCatalogController : ControllerBase
{
    private readonly IAdminCatalogService _service;

    public AdminCatalogController(IAdminCatalogService service)
    {
        _service = service;
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<AdminCategoryDto>>> GetCategories()
    {
        var result = await _service.GetCategoriesAsync();
        return Ok(result);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<AdminCategoryDto>> CreateCategory([FromBody] AdminCategoryCreateRequest request)
    {
        var result = await _service.CreateCategoryAsync(request);
        return Ok(result);
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<ActionResult<AdminCategoryDto>> UpdateCategory(Guid id, [FromBody] AdminCategoryUpdateRequest request)
    {
        var result = await _service.UpdateCategoryAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        await _service.DeleteCategoryAsync(id);
        return Ok();
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<AdminProductDto>>> GetProducts([FromQuery] string? search, [FromQuery] Guid? categoryId)
    {
        var result = await _service.GetProductsAsync(search, categoryId);
        return Ok(result);
    }

    [HttpPost("products")]
    public async Task<ActionResult<AdminProductDto>> CreateProduct([FromForm] AdminProductCreateRequest request)
    {
        var result = await _service.CreateProductAsync(request);
        return Ok(result);
    }

    [HttpPut("products/{id:guid}")]
    public async Task<ActionResult<AdminProductDto>> UpdateProduct(Guid id, [FromForm] AdminProductUpdateRequest request)
    {
        var result = await _service.UpdateProductAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        await _service.DeleteProductAsync(id);
        return Ok();
    }
}
