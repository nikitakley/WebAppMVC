using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Dto;

public class OrderStatusDto
{
    public int OrderStatusId { get; set; }

    [Required]
    public string? Status { get; set; }
}
