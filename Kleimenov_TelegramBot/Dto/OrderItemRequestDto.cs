namespace Kleimenov_TelegramBot.Dto;

public class OrderItemRequestDto
{
    public int OrderId { get; set; }
    public int DishId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
