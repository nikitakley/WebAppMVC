namespace Kleimenov_TelegramBot.Dto;

public class OrderItemDtoCustomer
{
    public int OrderItemId { get; set; }
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
