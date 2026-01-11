using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/promocodes")]
public class PromoCodesController : ControllerBase
{
    private readonly IPromoCodeService _promoCodes;

    public PromoCodesController(IPromoCodeService promoCodes)
    {
        _promoCodes = promoCodes;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<PromoCodeResult>> Validate([FromBody] ValidatePromoCodeRequest request)
    {
        var result = await _promoCodes.ValidateAsync(request);
        return Ok(result);
    }
}
