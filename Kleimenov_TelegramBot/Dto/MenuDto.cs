namespace Kleimenov_TelegramBot.Dto;

public class MenuDto
{
    public int RestaurantId {get;set;}
    public string Name { get; set; } = string.Empty;

    public List<DishDto> Dishes { get; set; } = new();
}
