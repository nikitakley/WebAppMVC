using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kleimenov_API.Dto;

public class RestaurantResponseDto
{
    public int RestaurantId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Column(TypeName = "decimal(4, 1)")]
    public decimal Rating { get; set; } = 0.0m;
}
