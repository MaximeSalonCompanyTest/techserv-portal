using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Extensions;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var user = await _db.Users.FindAsync(User.GetUserId());
        if (user == null)
        {
            return NotFound();
        }

        return Ok(ToResponse(user));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(User.GetUserId());
        if (user == null)
        {
            return NotFound();
        }

        user.Name = request.Name.Trim();
        user.Phone = request.Phone.Trim();
        await _db.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    private static ProfileResponse ToResponse(User user)
    {
        return new ProfileResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
