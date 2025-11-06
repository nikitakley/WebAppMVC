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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto authDto)
    {
        var token = await _authService.LoginJWT(authDto.Username, authDto.Password);
        return Ok(token);
    }
}
