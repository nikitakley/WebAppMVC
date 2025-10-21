using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Models;

public class Courier
{
    public int CourierId { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    public string? FullName { get; set; }

    [Required]
    public string? Phone { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
