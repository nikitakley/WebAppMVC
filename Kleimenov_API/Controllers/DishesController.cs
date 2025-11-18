using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/dishes")]
[Authorize]
public class DishesController : ControllerBase
{
    private readonly RestaurantDishService _restaurantDishService;

    public DishesController(RestaurantDishService restaurantDishService)
    {
        _restaurantDishService = restaurantDishService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dishes = await _restaurantDishService.GetAllDishesAsync();
        return Ok(dishes);
    }

    [HttpGet("availability")]
    public async Task<IActionResult> GetAllAvailable([FromQuery] bool? isAvailable)
    {
        var dishes = await _restaurantDishService.GetAllDishesByAvailabilityAsync(isAvailable);
        return Ok(dishes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDish(int id)
    {
        var dish = await _restaurantDishService.GetDishByIdAsync(id);
        if (dish == null)
            return NotFound();
        return Ok(dish);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateDish([FromBody] DishDto dishDto)
    {
        var created = await _restaurantDishService.CreateDishAsync(
            dishDto.RestaurantId,
            dishDto.Name,
            dishDto.Price,
            dishDto.IsAvailable
        );

        return CreatedAtAction(nameof(GetDish), new { id = created.DishId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDish(int id, [FromBody] DishDto dishDto)
    {
        await _restaurantDishService.UpdateDishAsync(
            id, dishDto.RestaurantId, dishDto.Name, dishDto.Price, dishDto.IsAvailable);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDish(int id)
    {
        await _restaurantDishService.DeleteDishAsync(id);
        return NoContent();
    }
}
