using System.ComponentModel.DataAnnotations;

namespace TechServ.Api.Dtos;

public class RegisterRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = "";

    [Required, Phone, MaxLength(30)]
    public string Phone { get; set; } = "";

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}
