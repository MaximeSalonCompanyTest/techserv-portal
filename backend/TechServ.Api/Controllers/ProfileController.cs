using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Extensions;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

// Any logged-in user, customer or agent. No token gives 401 before our code runs.
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
        // No ID in the URL: the profile is always the one of the user in the token.
        // FindAsync looks the user up by primary key.
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

        // Only name and phone can change: UpdateProfileRequest has no Email property, so an email sent anyway is ignored.
        user.Name = request.Name.Trim();
        user.Phone = request.Phone.Trim();
        await _db.SaveChangesAsync(); // EF Core sees which properties changed and sends an UPDATE for them

        return Ok(ToResponse(user));
    }

    // Copies the entity into the response DTO. The password hash is never sent back.
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
