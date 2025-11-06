using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kleimenov_API.Dto;

public record RestaurantDto(
    [Required] string Name,
    [Required] decimal Rating
);
