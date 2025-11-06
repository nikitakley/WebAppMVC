using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/dishes")]
[Authorize]
public class DishesController : ControllerBase
{
    private readonly DishService _dishService;

    public DishesController(DishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dishes = await _dishService.GetAllAsync();
        return Ok(dishes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDish(int id)
    {
        var dish = await _dishService.GetByIdAsync(id);
        if (dish == null)
            return NotFound();
        return Ok(dish);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateDish([FromBody] DishDto dishDto)
    {
        var created = await _dishService.CreateAsync(
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
        await _dishService.UpdateAsync(id, dishDto.RestaurantId, dishDto.Name, dishDto.Price, dishDto.IsAvailable);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDish(int id)
    {
        await _dishService.DeleteAsync(id);
        return NoContent();
    }
}
