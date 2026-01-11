using Identity.Models;
using Identity.Requets;
using Identity.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize]
public class AdminUsersController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public AdminUsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<AdminUserDto>>> GetUsers()
    {
        var hasAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Any();
        var isAdmin = User.IsInRole("Admin");
        if (hasAdmins && !isAdmin)
        {
            return Forbid();
        }

        var users = await _userManager.Users.ToListAsync();
        var result = new List<AdminUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Firstname = user.Firstname ?? string.Empty,
                Lastname = user.Lastname ?? string.Empty,
                Age = user.Age,
                Roles = roles.ToList()
            });
        }

        return Ok(result.OrderBy(u => u.Email));
    }

    [HttpPost("promote")]
    public async Task<ActionResult> Promote([FromBody] AdminUserRoleRequest request)
    {
        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required");
        }

        var hasAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Any();
        var isAdmin = User.IsInRole("Admin");
        if (hasAdmins && !isAdmin)
        {
            return Forbid();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }

    [HttpPost("demote")]
    public async Task<ActionResult> Demote([FromBody] AdminUserRoleRequest request)
    {
        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required");
        }

        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            return Forbid();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var isTargetAdmin = admins.Any(u => u.Id == user.Id);
        if (!isTargetAdmin)
        {
            return Ok();
        }

        if (admins.Count <= 1)
        {
            return BadRequest("Cannot remove the last admin");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}
