namespace Kleimenov_API.Dto;

public class OrderDtoCustomer
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int CourierId { get; set; }
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Вложенный список блюд
    public List<OrderItemDtoCustomer> Items { get; set; } = new();
}
