using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Kleimenov_API.Services;

public class OrderService
{
    private readonly Kleimenov_APIContext _context;

    public OrderService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public async Task<Order> CreateAsync(int customerId, int courierId, int restaurantId, int statusId)
    {
        var order = new Order
        {
            CustomerId = customerId,
            CourierId = courierId,
            RestaurantId = restaurantId,
            StatusId = statusId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateAsync(int orderId, int customerId, int courierId, int restaurantId, int statusId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.CustomerId = customerId;
            order.CourierId = courierId;
            order.RestaurantId = restaurantId;
            order.StatusId = statusId;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
