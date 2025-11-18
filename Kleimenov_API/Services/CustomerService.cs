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

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers.FindAsync(customerId);
    }

    public async Task<Customer> CreateCustomerAsync(string? fullName, string? email, string? phone, string? address)
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

    public async Task UpdateCustomerAsync(int customerId, string? fullName, string? email, string? phone, string? address)
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

    public async Task DeleteCustomerAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
