using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record DishDto(
    [Required] int RestaurantId,
    [Required] string Name,
    [Required] decimal Price,
    bool IsAvailable = true
);
