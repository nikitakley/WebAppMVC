using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class RestaurantDishService
{
    private readonly Kleimenov_APIContext _context;
    
    public RestaurantDishService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _context.Restaurants.ToListAsync();
    }

    public async Task<IEnumerable<Dish>> GetAllDishesAsync()
    {
        return await _context.Dishes.ToListAsync();
    }

    public async Task<IEnumerable<Dish>> GetAllDishesByAvailabilityAsync(bool? isAvailable)
    {
        var query = _context.Dishes.AsQueryable();

        if (isAvailable.HasValue)
            query = query.Where(d => d.IsAvailable == isAvailable.Value);
        return await query.ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int restaurantId)
    {
        return await _context.Restaurants.FindAsync(restaurantId);
    }

    public async Task<Dish?> GetDishByIdAsync(int dishId)
    {
        return await _context.Dishes.FindAsync(dishId);
    }

    public async Task<Restaurant?> GetMenuByIdAsync(int restaurantId)
    {
        return await _context.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
    }

    public async Task<Restaurant> CreateRestaurantAsync(string? name, decimal rating)
    {
        var restaurant = new Restaurant
        {
            Name = name ?? string.Empty,
            Rating = rating,
        };

        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();
        return restaurant;
    }

    public async Task<Dish> CreateDishAsync(int restaurantId, string? name, decimal price, bool availability)
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

    public async Task UpdateRestaurantAsync(int restaurantId, string? name, decimal rating)
    {
        var restaurant = await _context.Restaurants.FindAsync(restaurantId);
        if (restaurant != null)
        {
            restaurant.Name = name ?? string.Empty;
            restaurant.Rating = rating;

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateDishAsync(int dishId, int restaurantId, string? name, decimal price, bool availability)
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

    public async Task DeleteRestaurantAsync(int restaurantId)
    {
        var restaurant = await _context.Restaurants.FindAsync(restaurantId);
        if (restaurant != null)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteDishAsync(int dishId)
    {
        var dish = await _context.Dishes.FindAsync(dishId);
        if (dish != null)
        {
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }
    }
}
