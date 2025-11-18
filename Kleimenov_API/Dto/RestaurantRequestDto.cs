using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record RestaurantRequestDto(
    [Required] string Name,
    [Required] decimal Rating
);
