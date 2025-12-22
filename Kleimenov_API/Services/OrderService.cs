using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class OrderService
{
    private readonly Kleimenov_APIContext _context;

    public OrderService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.Dish)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderStatus>> GetAllOrderStatusesAsync()
    {
        return await _context.OrderStatuses.ToListAsync();
    }

    public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
    {
        return await _context.OrderItems.ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersByStatusIdAsync(int statusId)
    {
        return await _context.Orders
            .Where(o => o.StatusId == statusId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllOrdersByCustomerIdAsync(int customerId)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .Include(o => o.Status)
            .Include(o => o.Items)
                .ThenInclude(i => i.Dish)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FindAsync(orderId);
    }

    public async Task<OrderItem?> GetOrderItemByIdAsync(int orderItemId)
    {
        return await _context.OrderItems.FindAsync(orderItemId);
    }

    public async Task<IEnumerable<OrderItem>> GetOrderDetailsAsync(int orderId)
    {
        var items = await _context.OrderItems
            .Where(i => i.OrderId == orderId)
            //.Include(i => i.Dish)
            .ToListAsync();

        return items;
    }

    public async Task<Order> CreateOrderAsync(int customerId, int courierId, int restaurantId, int statusId)
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

    public async Task<OrderItem> CreateOrderItemAsync(int orderId, int dishId, int quantity, decimal unitPrice)
    {
        var item = new OrderItem
        {
            OrderId = orderId,
            DishId = dishId,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        _context.OrderItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task UpdateOrderAsync(int orderId, int customerId, int courierId, int restaurantId, int statusId)
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

    public async Task UpdateOrderItemAsync(int id, int quantity)
    {
        var item = await _context.OrderItems.FindAsync(id);
        if (item != null)
        {
            item.Quantity = quantity;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteOrderItemAsync(int orderItemId)
    {
        var item = await _context.OrderItems.FindAsync(orderItemId);
        if (item != null)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
