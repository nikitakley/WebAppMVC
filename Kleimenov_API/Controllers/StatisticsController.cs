using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.DtoStatistics;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[Route("api/statistics")]
[ApiController]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("top/ordered")]
    public async Task<IActionResult> GetTopDish()
    {
        return Ok(await _statisticsService.GetTopDishAsync());
    }

    [HttpGet("top/profit")]
    public async Task<IActionResult> GetMostProfitableDish()
    {
        return Ok(await _statisticsService.GetMostProfitableDishAsync());
    }
}
