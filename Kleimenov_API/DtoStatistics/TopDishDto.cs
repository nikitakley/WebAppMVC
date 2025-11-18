namespace Kleimenov_API.DtoStatistics;

public class TopDishDto
{
    public int DishId { get; set; }
    public string DishName { get; set; } = string.Empty;
    public int TotalOrdered { get; set; }
}
