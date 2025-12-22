using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Models;

public class User
{
    public int UserId { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User";

    // связь с профилем клиента
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
