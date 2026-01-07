namespace Kleimenov_TelegramBot.Dto;

public class DishDto
{
    public int DishId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}