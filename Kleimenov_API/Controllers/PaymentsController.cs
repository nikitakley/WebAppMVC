using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Microsoft.AspNetCore.Authorization;


namespace Kleimenov_API.Controllers;

[ApiController]
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
        var payments = await _paymentService.GetAllPaymentsAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetPaymentByOrderId(int orderId)
    {
        var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
        if (payment == null)
            return NotFound();
        return Ok(payment);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("order/{orderId}")]
    public async Task<IActionResult> CreatePayment(int orderId)
    {
        try
        {
            var created = await _paymentService.CreatePaymentAsync(orderId);
            return CreatedAtAction(nameof(GetPayment), new { id = created.PaymentId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
