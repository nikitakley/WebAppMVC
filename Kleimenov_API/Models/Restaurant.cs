using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kleimenov_API.Models;

public class Restaurant
{
    public int RestaurantId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(4, 1)")]
    public decimal Rating { get; set; } = 0.0m;
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
