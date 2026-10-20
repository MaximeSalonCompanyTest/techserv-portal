namespace TechServ.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = ""; // only the salted hash is stored, never the password itself
    public UserRole Role { get; set; } // Customer or Agent, copied into the token as the role claim
    public DateTime CreatedAt { get; set; }
}
