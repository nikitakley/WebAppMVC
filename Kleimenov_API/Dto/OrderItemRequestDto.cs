namespace Kleimenov_API.Dto;

public record OrderItemRequestDto
(
    int OrderId,
    int DishId,
    int Quantity,
    decimal UnitPrice
);
