using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class CourierService
{
    private readonly Kleimenov_APIContext _context;

    public CourierService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Courier>> GetAllAsync()
    {
        return await _context.Couriers
            .Include(c => c.Orders)
            .ToListAsync();
    }

    public async Task<Courier?> GetByIdAsync(int courierId)
    {
        return await _context.Couriers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CourierId == courierId);
    }

    public async Task<Courier> CreateAsync(string? fullName, string? phone)
    {
        var courier = new Courier
        {
            FullName = fullName,
            Phone = phone
        };

        _context.Couriers.Add(courier);
        await _context.SaveChangesAsync();
        return courier;
    }

    public async Task UpdateAsync(int courierId, string? fullName, string? phone)
    {
        var courier = await _context.Couriers.FindAsync(courierId);
        if (courier != null)
        {
            courier.FullName = fullName;
            courier.Phone = phone;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int courierId)
    {
        var courier = await _context.Couriers.FindAsync(courierId);
        if (courier != null)
        {
            _context.Couriers.Remove(courier);
            await _context.SaveChangesAsync();
        }
    }
}
