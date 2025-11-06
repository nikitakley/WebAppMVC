using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record CourierDto(
    [Required] string FullName,
    [Required] string Phone
);
