using Identity.Requets;
using Identity.Responses;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IWebHostEnvironment _environment;

        public IdentityController(IIdentityService identityService, IWebHostEnvironment environment)
        {
            _identityService = identityService;
            _environment = environment;
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _identityService.ResetPassword(request);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _identityService.ChangePassword(request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ConfirmResetPassword")]
        public async Task<ActionResult<ConfirmResetPasswordResponse>> ConfirmResetPassword([FromBody] ConfirmResetPasswordRequest request)
        {
            try
            {
                var result = await _identityService.ConfirmResetPassword(request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest request)
        {
            try
            {
                var result = await _identityService.Login(request);
                SetRefreshCookie(result.RefreshToken, request?.RememberMe ?? false);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserRegisterResponse>> Register([FromBody] UserRegisterRequest request)
        {
            try
            {
                var result = await _identityService.Register(request);
                SetRefreshCookie(result.RefreshToken, request.RememberMe);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Me")]
        [Authorize]
        public ActionResult GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);

            return Ok(new
            {
                userId,
                email,
                name
            });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        {
            try
            {
                await _identityService.ConfirmEmail(request);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest? request)
        {
            try
            {
                var token = string.IsNullOrWhiteSpace(request?.Token)
                    ? Request.Cookies["refreshToken"]
                    : request?.Token;
                if (string.IsNullOrWhiteSpace(token))
                {
                    return Unauthorized();
                }
                var result = await _identityService.RefreshToken(new RefreshTokenRequest
                {
                    Token = token,
                    RememberMe = request?.RememberMe ?? false
                });
                SetRefreshCookie(result.RefreshToken, request.RememberMe);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest? request)
        {
            var token = string.IsNullOrWhiteSpace(request?.Token)
                ? Request.Cookies["refreshToken"]
                : request?.Token;
            if (!string.IsNullOrWhiteSpace(token))
            {
                await _identityService.Logout(new LogoutRequest { Token = token });
            }
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        private void SetRefreshCookie(string refreshToken, bool rememberMe)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = !_environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            if (rememberMe)
            {
                options.Expires = DateTimeOffset.UtcNow.AddDays(30);
            }
            Response.Cookies.Append("refreshToken", refreshToken, options);
        }

    }
}
