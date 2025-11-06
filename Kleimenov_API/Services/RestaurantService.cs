using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class RestaurantService
{
    private readonly Kleimenov_APIContext _context;

    public RestaurantService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        return await _context.Restaurants
            .Include(r => r.Dishes)
            .ToListAsync();
    }

    public async Task<Restaurant?> GetByIdAsync(int restaurantId)
    {
        return await _context.Restaurants
            .Include(r => r.Dishes)
            .Include(r => r.Orders)
            .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
    }

    public async Task<Restaurant?> GetByIdMenuAsync(int restaurantId, bool availableOnly = false)
    {
        var restaurant = await _context.Restaurants
            .Where(r => r.RestaurantId == restaurantId)
            .Select(r => new Restaurant
            {
                RestaurantId = r.RestaurantId,
                Name = r.Name,
                Rating = r.Rating,
                Dishes = r.Dishes
                        .Where(d => !availableOnly || d.IsAvailable)
                        .Select(d => new Dish
                        {
                            DishId = d.DishId,
                            Name = d.Name,
                            Price = d.Price,
                            IsAvailable = d.IsAvailable
                        }).ToList()
            })
            .FirstOrDefaultAsync();

        return restaurant;
    }

    public async Task<Restaurant> CreateAsync(string? name, decimal rating)
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

    public async Task UpdateAsync(int restaurantId, string? name, decimal rating)
    {
        var restaurant = await _context.Restaurants.FindAsync(restaurantId);
        if (restaurant != null)
        {
            restaurant.Name = name ?? string.Empty;
            restaurant.Rating = rating;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int restaurantId)
    {
        var restaurant = await _context.Restaurants.FindAsync(restaurantId);
        if (restaurant != null)
        {
            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();
        }
    }
}
