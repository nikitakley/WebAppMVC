using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record OrderItemDto
(
    int OrderId,
    int DishId,
    int Quantity,
    decimal UnitPrice
);
