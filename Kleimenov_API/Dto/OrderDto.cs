using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record OrderDto
(
    int CustomerId,
    int CourierId,
    int RestaurantId,
    int StatusId
);
