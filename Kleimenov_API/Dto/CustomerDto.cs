using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public class CustomerDto
{
    [Required]
    public string? FullName { get; set; }

    [Required]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Address { get; set; }
}
