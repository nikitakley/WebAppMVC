using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kleimenov_API.Models;

public class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Display(Name = "Paid At")]
    public DateTime PaidAt { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;
}
