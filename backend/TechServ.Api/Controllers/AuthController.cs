using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TechServ.Api.Data;
using TechServ.Api.Dtos;
using TechServ.Api.Models;

namespace TechServ.Api.Controllers;

// No [Authorize] here: these endpoints are how a user gets a token in the first place.
// [ApiController] checks the DTO's rules ([Required], [EmailAddress]...) and answers 400 before our code runs.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    // Built into ASP.NET Core Identity: adds a random salt and hashes the password, so the real password is never stored.
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Same email typed as "Ana@Mail.com " or "ana@mail.com" must count as one account.
        var email = request.Email.Trim().ToLowerInvariant();

        // Friendly 409 here. The unique index on Email in the database is the last guard
        // if two sign-ups with the same email arrive at the same moment.
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            return Conflict(new { message = "This email is already registered." });
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Role = UserRole.Customer, // sign-up always creates a customer; agent accounts only come from the seed data
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // One message for both cases, so nobody can use the login to find out which emails have accounts.
        if (user == null ||
            _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(new LoginResponse
        {
            Token = CreateToken(user),
            Name = user.Name,
            Role = user.Role.ToString()
        });
    }

    private string CreateToken(User user)
    {
        // Claims are the facts the API trusts later without asking the database: who (ID), email and role.
        // Anyone can read a JWT, so it holds nothing secret: no password, no phone.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // Signed with a secret key only the server has. If anyone edits the token (for example the role),
        // the signature no longer matches and the API rejects it.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // logout deletes the token in the browser; a copied token still works until this expiry
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
