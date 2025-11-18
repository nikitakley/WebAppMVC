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

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
    {
        return await _context.Payments.FindAsync(paymentId);
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        return payment;
    }

    public async Task<Payment> CreatePaymentAsync(int orderId)
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
