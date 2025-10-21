using System.ComponentModel.DataAnnotations.Schema;

namespace Kleimenov_API.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int DishId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(DishId))]
    public Dish Dish { get; set; } = null!;
}
