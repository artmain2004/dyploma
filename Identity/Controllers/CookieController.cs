using Identity.Models;
using Identity.Requets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]


public class CookieController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public CookieController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;

        _signInManager.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.PasswordSignInAsync(
            userName: user.UserName!,
            password: request.Password,
            isPersistent: true,      
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Unauthorized("User locked out");

        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        return Ok();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return BadRequest("User with this email already exists");

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            Firstname = request.Firstname,
            Lastname = request.Lastname
        };

        var create = await _userManager.CreateAsync(user, request.Password);
        if (!create.Succeeded)
            return BadRequest(create.Errors);

        var addRole = await _userManager.AddToRoleAsync(user, "User");
        if (!addRole.Succeeded)
            return BadRequest(addRole.Errors);

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpGet("me")]
    public IActionResult Me()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Unauthorized();

        return Ok(new
        {
            Name = User.Identity.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
