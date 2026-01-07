namespace Kleimenov_TelegramBot.Dto;

public class OrderRequestDto
{
    public int CustomerId { get; set; }
    public int CourierId { get; set; } = 1;  // назначение 1го курьера
    public int RestaurantId { get; set; }
    public int StatusId { get; set; } = 1;  // "Создан"
}
