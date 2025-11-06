using Kleimenov_API.Data;
using Kleimenov_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kleimenov_API.Services;

public class PaymentService
{
    private readonly Kleimenov_APIContext _context;

    public PaymentService(Kleimenov_APIContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
            .ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(int paymentId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        return await _context.Payments
            .Include(p => p.Order)
            .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<Payment> CreateAsync(int orderId)
    {
        bool exists = await _context.Payments.AnyAsync(p => p.OrderId == orderId);
        if (exists)
            throw new InvalidOperationException($"Оплата для заказа #{orderId} уже существует.");

        var total = await _context.OrderItems
            .Where(i => i.OrderId == orderId)
            .SumAsync(i => i.Quantity * i.UnitPrice);

        var payment = new Payment
        {
            OrderId = orderId,
            Amount = total,
            PaidAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return payment;
    }
}
