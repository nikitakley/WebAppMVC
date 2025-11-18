using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/statuses")]
[Authorize]
public class OrderStatusesController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderStatusesController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orderStatuses = await _orderService.GetAllOrderStatusesAsync();

        var response = orderStatuses.Select(orderStatus => new OrderStatusDto
        {
            OrderStatusId = orderStatus.OrderStatusId,
            Status = orderStatus.Status
        }).ToList();
        return Ok(response);
    }
}