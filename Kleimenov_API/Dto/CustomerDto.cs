using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public record CustomerDto(
    [Required] string FullName,
    [EmailAddress] string? Email,
    [Required] string Phone,
    [Required] string Address
);
