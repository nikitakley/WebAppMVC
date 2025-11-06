using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class OrderItemService
{
    private readonly Kleimenov_APIContext _context;

    public OrderItemService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    //public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int? orderId = null)
    //{
    //    var query = _context.OrderItems
    //        .Include(i => i.Dish)
    //        .Include(i => i.Order)
    //        .AsQueryable();

    //    if (orderId.HasValue)
    //        query = query.Where(i => i.OrderId == orderId.Value);

    //    return await query.ToListAsync();
    //}

    public async Task<IEnumerable<OrderItem>> GetAllAsync()
    {
        return await _context.OrderItems.ToListAsync();
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await _context.OrderItems
            .Include(i => i.Dish)
            .Include(i => i.Order)
            .FirstOrDefaultAsync(i => i.OrderItemId == id);
    }

    public async Task<OrderItem> CreateAsync(int orderId, int dishId, int quantity, decimal unitPrice)
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

    public async Task UpdateAsync(int id, int quantity, decimal unitPrice)
    {
        var item = await _context.OrderItems.FindAsync(id);
        if (item != null)
        {
            item.Quantity = quantity;
            item.UnitPrice = unitPrice;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.OrderItems.FindAsync(id);
        if (item != null)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<object>> GetItemsByOrderAsync(int orderId)
    {
        var items = await _context.OrderItems
            .Where(i => i.OrderId == orderId)
            .Select(i => new
            {
                i.OrderItemId,
                DishName = i.Dish.Name,
                i.Quantity,
                i.UnitPrice,
                Total = i.UnitPrice * i.Quantity
            })
            .ToListAsync();

        return items;
    }
}
