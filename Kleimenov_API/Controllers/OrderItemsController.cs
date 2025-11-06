using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/order-items")]
[Authorize]
public class OrderItemsController : ControllerBase
{
    private readonly OrderItemService _orderItemService;

    public OrderItemsController(OrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    //[HttpGet("by-order-id")]
    //public async Task<IActionResult> GetByOrderId([FromQuery] int? orderId)
    //{
    //    var items = await _orderItemService.GetByOrderIdAsync(orderId);
    //    return Ok(items);
    //}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderItems = await _orderItemService.GetAllAsync();
        return Ok(orderItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        var item = await _orderItemService.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        return Ok(item);
    }

    //GET /api/order-items/by-order/{orderId}
    [HttpGet("by-order/{orderId}")]
    public async Task<IActionResult> GetItemsByOrder(int orderId)
    {
        var items = await _orderItemService.GetItemsByOrderAsync(orderId);
        return Ok(items);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateOrderItem([FromBody] OrderItemDto dto)
    {
        var created = await _orderItemService.CreateAsync(
            dto.OrderId, 
            dto.DishId, 
            dto.Quantity, 
            dto.UnitPrice
        );

        return CreatedAtAction(nameof(GetOrderItem), new { id = created.OrderItemId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] OrderItemResponseDto dto)
    {
        await _orderItemService.UpdateAsync(id, dto.Quantity, dto.UnitPrice);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        await _orderItemService.DeleteAsync(id);
        return NoContent();
    }
}
