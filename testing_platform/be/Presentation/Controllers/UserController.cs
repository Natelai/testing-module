using System.Runtime.CompilerServices;
using DTOs.Requests.Users;
using DTOs.Responses.Users;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

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

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.UserName = request.UserName;

        await _context.SaveChangesAsync(ct);

        return Ok();
    }
}