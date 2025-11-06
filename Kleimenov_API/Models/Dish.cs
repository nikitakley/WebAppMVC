using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Kleimenov_API.Models;

public class Dish
{
    public int DishId { get; set; }

    public int RestaurantId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Display(Name = "Is Available")]
    public bool IsAvailable { get; set; } = true;

    [JsonIgnore]
    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; } = null!;

    [JsonIgnore]
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
