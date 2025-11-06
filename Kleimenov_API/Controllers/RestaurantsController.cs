using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/restaurants")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantService _restaurantService;

    public RestaurantsController(RestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restaurants = await _restaurantService.GetAllAsync();
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurant(int id)
    {
        var restaurant = await _restaurantService.GetByIdAsync(id);
        if (restaurant == null)
            return NotFound();
        return Ok(restaurant);
    }

    [HttpGet("{id}/{availability}")]
    public async Task<IActionResult> GetRestaurantMenu(int id, bool availability)
    {
        var restaurant = await _restaurantService.GetByIdMenuAsync(id, availability);
        if (restaurant == null)
            return NotFound();
        return Ok(restaurant);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantDto restaurantDto)
    {
        var created = await _restaurantService.CreateAsync(
            restaurantDto.Name,
            restaurantDto.Rating
        );

        return CreatedAtAction(nameof(GetRestaurant), new { id = created.RestaurantId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] RestaurantDto restaurantDto)
    {
        await _restaurantService.UpdateAsync(id, restaurantDto.Name, restaurantDto.Rating);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        await _restaurantService.DeleteAsync(id);
        return NoContent();
    }
}
