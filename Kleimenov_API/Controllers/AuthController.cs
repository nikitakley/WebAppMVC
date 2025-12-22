using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;

namespace Kleimenov_API.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _authService.RegisterAsync(registerDto.Username, registerDto.Password, registerDto.FullName, registerDto.Phone, registerDto.Email, registerDto.Address);
        if (!success)
            return BadRequest("Пользователь с таким именем уже существует.");

        return Ok("Регистрация прошла успешно.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto authDto)
    {
        var (token, customer, role) = await _authService.LoginAsync(authDto.Username, authDto.Password);

        if (token == null || customer == null)
            return Unauthorized("Неверное имя пользователя или пароль.");

        return Ok(new
        {
            token,
            customer = new
            {
                customer.CustomerId,
                customer.FullName,
                customer.Phone,
                customer.Email,
                customer.Address,
                customer.RegisteredAt,
                role
            }
        });
    }
}
