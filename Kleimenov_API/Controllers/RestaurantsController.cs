using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/restaurants")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantDishService _restaurantDishService;

    public RestaurantsController(RestaurantDishService restaurantDishService)
    {
        _restaurantDishService = restaurantDishService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var restaurants = await _restaurantDishService.GetAllRestaurantsAsync();

        var response = restaurants.Select(restaurant => new RestaurantResponseDto
        {
            RestaurantId = restaurant.RestaurantId,
            Name = restaurant.Name,
            Rating = restaurant.Rating
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurant(int id)
    {
        var restaurant = await _restaurantDishService.GetRestaurantByIdAsync(id);
        if (restaurant == null)
            return NotFound();

        var response = new RestaurantResponseDto
        {
            RestaurantId = restaurant.RestaurantId,
            Name = restaurant.Name,
            Rating = restaurant.Rating
        };
        return Ok(response);
    }

    [HttpGet("{id}/menu")]
    public async Task<IActionResult> GetRestaurantMenu(int id)
    {
        var restaurant = await _restaurantDishService.GetMenuByIdAsync(id);
        if (restaurant == null)
            return NotFound();
        return Ok(restaurant);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantRequestDto restaurantDto)
    {
        var created = await _restaurantDishService.CreateRestaurantAsync(
            restaurantDto.Name,
            restaurantDto.Rating
        );

        return CreatedAtAction(nameof(GetRestaurant), new { id = created.RestaurantId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] RestaurantRequestDto restaurantDto)
    {
        await _restaurantDishService.UpdateRestaurantAsync(id, restaurantDto.Name, restaurantDto.Rating);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        await _restaurantDishService.DeleteRestaurantAsync(id);
        return NoContent();
    }
}
