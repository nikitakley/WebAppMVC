using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;


namespace Kleimenov_API.Services;

public class OrderStatusService
{
    private readonly Kleimenov_APIContext _context;

    public OrderStatusService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderStatus>> GetAllAsync()
    {
        return await _context.OrderStatuses.ToListAsync();
    }

    //public async Task<OrderStatus?> GetByIdAsync(int orderStatusId)
    //{
    //    return await _context.OrderStatuses.FindAsync(orderStatusId);
    //}

    //public async Task<OrderStatus> CreateAsync(string? status)
    //{
    //    var orderStatus = new OrderStatus
    //    {
    //        Status = status
    //    };

    //    _context.OrderStatuses.Add(orderStatus);
    //    await _context.SaveChangesAsync();
    //    return orderStatus;
    //}

    //public async Task UpdateAsync(int orderStatusId, string? status)
    //{
    //    var orderStatus = await _context.OrderStatuses.FindAsync(orderStatusId);
    //    if (orderStatus != null)
    //    {
    //        orderStatus.Status = status;

    //        await _context.SaveChangesAsync();
    //    }
    //}

    //public async Task DeleteAsync(int orderStatusId)
    //{
    //    var orderStatus = await _context.OrderStatuses.FindAsync(orderStatusId);
    //    if (orderStatus != null)
    //    {
    //        _context.OrderStatuses.Remove(orderStatus);
    //        await _context.SaveChangesAsync();
    //    }
    //}
}

