using System.Runtime.CompilerServices;
using Domain.dbo;
using DTOs.Requests.Users;
using DTOs.Responses.Users;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager = roleManager;

    [HttpGet("short-profile")]
    public async Task<ActionResult<ShortProfileUserDto>> GetShortUserProfile(CancellationToken ct)
    {
        var userName = User.Identity!.Name;

        if (userName is null)
        {
            return BadRequest("Unable to check user identity");
        }

        var rsp = await _context.Users.Select(u => new ShortProfileUserDto()
        {
            Email = u.Email!,
            UserName = u.UserName ?? userName!,
        }).SingleAsync(u => u.UserName == userName, ct);

        return Ok(rsp);
    }

    [HttpGet("full-profile")]
    public async Task<ActionResult<FullProfileUserDto>> GetFullUserProfile(CancellationToken ct)
    {
        var userName = User.Identity!.Name;

        if (userName is null)
        {
            return BadRequest("Unable to check user identity");
        }

        var rsp = await  _context.Users.Select(u => new FullProfileUserDto()
        {
            Email = u.Email!,
            UserName = u.UserName ?? userName!,
            FirstName = u.FirstName,
            LastName = u.LastName,
        }).SingleAsync(u => u.UserName == userName, ct);

        return Ok(rsp);
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest request, CancellationToken ct)
    {
        var userName = User.Identity!.Name;

        if (userName is null)
        {
            return BadRequest("Unable to check user identity");
        }

        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName, ct);

        if (user is null)
        {
            return BadRequest("Unable to find linked user account");
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.Email);
        await _userManager.ChangeEmailAsync(user, request.Email, token);
        await _userManager.SetUserNameAsync(user, request.UserName);

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        await _context.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpGet("premium")]
    public async Task<ActionResult> UpdateUserToPremium(CancellationToken ct)
    {
        var userName = User.Identity!.Name;

        if (userName is null)
        {
            return BadRequest("Unable to check user identity");
        }

        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName, ct);

        if (user is null)
        {
            return BadRequest("Unable to find linked user account");
        }

        if (user.IsPremium)
        {
            return BadRequest("User already Premium");
        }

        user.IsPremium = true;
        await _context.SaveChangesAsync(ct);

        return Ok();
    }

    [HttpGet("isPremium")]
    public async Task<ActionResult> IsUserPremium(CancellationToken ct)
    {
        var userName = User.Identity!.Name;

        if (userName is null)
        {
            return BadRequest("Unable to check user identity");
        }

        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName, ct);

        if (user.IsPremium)
        {
            return Ok();
        }

        return BadRequest();
    }
}