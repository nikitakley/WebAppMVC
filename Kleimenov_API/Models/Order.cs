using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Kleimenov_API.Models;
public class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int CourierId { get; set; }

    public int RestaurantId { get; set; }

    public int StatusId { get; set; }

    //[Column(TypeName = "decimal(10, 2)")]
    //public decimal TotalPrice { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(CustomerId))]
    public Customer Customer { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(CourierId))]
    public Courier Courier { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; } = null!;

    [JsonIgnore]
    [ForeignKey(nameof(StatusId))]
    public OrderStatus Status { get; set; } = null!;

    [JsonIgnore]
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    [JsonIgnore]
    public Payment? Payment { get; set; }
    //public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
