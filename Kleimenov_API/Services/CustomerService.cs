using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class CustomerService
{
    private readonly Kleimenov_APIContext _context;

    public CustomerService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.Orders)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        //return await _context.Customers.FindAsync(customerId);
        return await _context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Customer> CreateAsync(string? fullName, string? email, string? phone, string? address)
    {
        var customer = new Customer
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            Address = address ?? string.Empty,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task UpdateAsync(int customerId, string? fullName, string? email, string? phone, string? address)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.FullName = fullName;
            customer.Email = email;
            customer.Phone = phone;
            customer.Address = address ?? string.Empty;

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
