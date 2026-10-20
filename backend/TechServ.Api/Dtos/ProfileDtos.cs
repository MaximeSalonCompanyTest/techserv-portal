using System.ComponentModel.DataAnnotations;

namespace TechServ.Api.Dtos;

// No PasswordHash property, so the hash can never be sent to the browser.
public class ProfileResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

// No Email property: that is what keeps the email read-only.
public class UpdateProfileRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, Phone, MaxLength(30)]
    public string Phone { get; set; } = "";
}
