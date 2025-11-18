using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("status/{statusId}")]
    public async Task<IActionResult> GetAllOrdersByStatusId(int statusId)
    {
        var orders = await _orderService.GetAllOrdersByStatusIdAsync(statusId);
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails(int id)
    {
        var items = await _orderService.GetOrderDetailsAsync(id);

        var result = items.Select(i => new OrderItemResponseDto
        {
            OrderItemId = i.OrderItemId,
            OrderId = i.OrderId,
            DishId = i.DishId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            Total = i.UnitPrice * i.Quantity
        });
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
    {
        var created = await _orderService.CreateOrderAsync(
            orderDto.CustomerId,
            orderDto.CourierId,
            orderDto.RestaurantId,
            orderDto.StatusId
        );

        return CreatedAtAction(nameof(GetOrder), new { id = created.OrderId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto orderDto)
    {
        await _orderService.UpdateOrderAsync(id, orderDto.CustomerId, orderDto.CourierId, orderDto.RestaurantId, orderDto.StatusId);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _orderService.DeleteOrderAsync(id);
        return NoContent();
    }
}
