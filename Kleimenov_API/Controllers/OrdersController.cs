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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();

        var orderDtos = orders.Select(o => new OrderDtoCustomer
        {
            OrderId = o.OrderId,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer.FullName,
            CourierId = o.CourierId,
            RestaurantId = o.RestaurantId,
            RestaurantName = o.Restaurant.Name,
            StatusId = o.StatusId,
            StatusName = o.Status.Status,
            CreatedAt = o.CreatedAt,
            Items = o.Items.Select(i => new OrderItemDtoCustomer
            {
                OrderItemId = i.OrderItemId,
                DishId = i.DishId,
                DishName = i.Dish.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();

        return Ok(orderDtos);
    }
    //public async Task<IActionResult> GetAll()
    //{
    //    var orders = await _orderService.GetAllOrdersAsync();
    //    return Ok(orders);
    //}

    [HttpGet("status/{statusId}")]
    public async Task<IActionResult> GetAllOrdersByStatusId(int statusId)
    {
        var orders = await _orderService.GetAllOrdersByStatusIdAsync(statusId);
        return Ok(orders);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GettAllOrdersByCustomerId(int customerId)
    {
        var orders = await _orderService.GetAllOrdersByCustomerIdAsync(customerId);

        var orderDtos = orders.Select(o => new OrderDtoCustomer
        {
            OrderId = o.OrderId,
            CustomerId = o.CustomerId,
            CustomerName = o.Customer.FullName,
            CourierId = o.CourierId,
            RestaurantId = o.RestaurantId,
            RestaurantName = o.Restaurant.Name,
            StatusId = o.StatusId,
            StatusName = o.Status.Status,
            CreatedAt = o.CreatedAt,
            Items = o.Items.Select(i => new OrderItemDtoCustomer
            {
                OrderItemId = i.OrderItemId,
                DishId = i.DishId,
                DishName = i.Dish.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();

        return Ok(orderDtos);
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
