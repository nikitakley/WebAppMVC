using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class DishService
{
    private readonly Kleimenov_APIContext _context;

    public DishService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dish>> GetAllAsync()
    {
        return await _context.Dishes.ToListAsync();
    }

    public async Task<Dish?> GetByIdAsync(int dishId)
    {
        return await _context.Dishes.FindAsync(dishId);
    }

    public async Task<Dish> CreateAsync(int restaurantId, string? name, decimal price, bool availability)
    {
        var dish = new Dish
        {
            RestaurantId = restaurantId,
            Name = name,
            Price = price,
            IsAvailable = availability
        };

        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync();
        return dish;
    }

    public async Task UpdateAsync(int dishId, int restaurantId, string? name, decimal price, bool availability)
    {
        var dish = await _context.Dishes.FindAsync(dishId);
        if (dish != null)
        {
            dish.RestaurantId = restaurantId;
            dish.Name = name;
            dish.Price = price;
            dish.IsAvailable = availability;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int dishId)
    {
        var dish = await _context.Dishes.FindAsync(dishId);
        if (dish != null)
        {
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }
    }
}
