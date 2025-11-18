using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/couriers")]
[Authorize]
public class CouriersController : ControllerBase
{
    private readonly CourierService _courierService;

    public CouriersController(CourierService courierService)
    {
        _courierService = courierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var couriers = await _courierService.GetAllCouriersAsync();
        return Ok(couriers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourier(int id)
    {
        var courier = await _courierService.GetCourierByIdAsync(id);
        if (courier == null)
            return NotFound();
        return Ok(courier);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCourier([FromBody] CourierDto courierDto)
    {
        var created = await _courierService.CreateAsync(
            courierDto.FullName,
            courierDto.Phone
        );

        return CreatedAtAction(nameof(GetCourier), new { id = created.CourierId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourier(int id, [FromBody] CourierDto courierDto)
    {
        await _courierService.UpdateAsync(id, courierDto.FullName, courierDto.Phone);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourier(int id)
    {
        await _courierService.DeleteAsync(id);
        return NoContent();
    }
}
