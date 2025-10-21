using System.ComponentModel.DataAnnotations;

namespace Kleimenov_API.Models;

public class OrderStatus
{
    public int OrderStatusId { get; set; }

    [Required]
    public string? Status { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
