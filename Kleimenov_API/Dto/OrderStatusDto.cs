using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record OrderStatusDto(
    int OrderStatusId,
    string Status
);