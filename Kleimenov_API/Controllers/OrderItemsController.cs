using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/items")]
[Authorize]
public class OrderItemsController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderItemsController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderItems = await _orderService.GetAllOrderItemsAsync();
        return Ok(orderItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderItem(int id)
    {
        var orderItem = await _orderService.GetOrderItemByIdAsync(id);
        if (orderItem == null)
            return NotFound();
        return Ok(orderItem);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateOrderItem([FromBody] OrderItemRequestDto dto)
    {
        var created = await _orderService.CreateOrderItemAsync(
            dto.OrderId, 
            dto.DishId, 
            dto.Quantity, 
            dto.UnitPrice
        );

        return CreatedAtAction(nameof(GetOrderItem), new { id = created.OrderItemId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrderItem(int id, [FromBody] OrderItemRequestQuantityDto dto)
    {
        await _orderService.UpdateOrderItemAsync(id, dto.Quantity);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrderItem(int id)
    {
        await _orderService.DeleteOrderItemAsync(id);
        return NoContent();
    }
}
