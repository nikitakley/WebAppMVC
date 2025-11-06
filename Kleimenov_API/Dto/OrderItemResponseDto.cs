using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record OrderItemResponseDto
(
    int Quantity,
    decimal UnitPrice
);
