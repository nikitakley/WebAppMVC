using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Microsoft.AspNetCore.Authorization;


namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payments = await _paymentService.GetAllAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [HttpGet("by-order/{orderId}")]
    public async Task<IActionResult> GetByOrderId(int orderId)
    {
        var payment = await _paymentService.GetByOrderIdAsync(orderId);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{orderId}")]
    public async Task<IActionResult> CreatePayment(int orderId)
    {
        try
        {
            var created = await _paymentService.CreateAsync(orderId);
            return CreatedAtAction(nameof(GetPayment), new { id = created.PaymentId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
