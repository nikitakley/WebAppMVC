using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [Required]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Registered At")]
    public DateTime RegisteredAt { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
