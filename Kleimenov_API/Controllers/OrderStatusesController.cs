using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/order-statuses")]
[Authorize]
public class OrderStatusesController : ControllerBase
{
    private readonly OrderStatusService _orderStatusService;

    public OrderStatusesController(OrderStatusService orderStatusService)
    {
        _orderStatusService = orderStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderStatuses = await _orderStatusService.GetAllAsync();

        var dtos = orderStatuses.Select(s => new OrderStatusDto(
            s.OrderStatusId,
            s.Status ?? string.Empty
        ));
        
        return Ok(dtos);
    }

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetStatus(int id)
    //{
    //    var orderStatus = await _orderStatusService.GetByIdAsync(id);
    //    if (orderStatus == null)
    //        return NotFound();
    //    return Ok(orderStatus);
    //}

    //[HttpPost]
    //public async Task<IActionResult> CreateStatus([FromBody] OrderStatusDto orderStatusDto)
    //{
    //    var created = await _orderStatusService.CreateAsync(
    //        orderStatusDto.Status
    //    );

    //    return CreatedAtAction(nameof(GetStatus), new { id = created.OrderStatusId }, created);
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusDto orderStatusDto)
    //{
    //    await _orderStatusService.UpdateAsync(id, orderStatusDto.Status);
    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteStatus(int id)
    //{
    //    await _orderStatusService.DeleteAsync(id);
    //    return NoContent();
    //}
}
