using Kleimenov_API.Data;
using Kleimenov_API.DtoStatistics;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class StatisticsService
{
    private readonly Kleimenov_APIContext _context;

    public StatisticsService(Kleimenov_APIContext context)
    {
        _context = context;
    }
    public async Task<TopDishDto?> GetTopDishAsync()
    {
        return await _context.OrderItems
            .GroupBy(i => new { i.DishId, i.Dish.Name })
            .Select(g => new TopDishDto
            {
                DishId = g.Key.DishId,
                DishName = g.Key.Name,
                TotalOrdered = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalOrdered)
            .FirstOrDefaultAsync();
    }

    public async Task<ProfitDishDto?> GetMostProfitableDishAsync()
    {
        return await _context.OrderItems
            .GroupBy(i => new { i.DishId, i.Dish.Name })
            .Select(g => new ProfitDishDto
            {
                DishId = g.Key.DishId,
                DishName = g.Key.Name,
                Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
            })
            .OrderByDescending(x => x.Revenue)
            .FirstOrDefaultAsync();
    }
}
