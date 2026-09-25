using IlliaUlianych_APB_TZ.Services;
using Microsoft.AspNetCore.Mvc;

namespace IlliaUlianych_APB_TZ.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("income")]
    public async Task<IActionResult> GetIncome(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (endDate <= startDate)
        {
            return BadRequest("End date must be later than start date.");
        }

        var report = await _reportService.GetIncomeAsync(startDate, endDate);
        return Ok(report);
    }
    
    [HttpGet("room-usage")]
    public async Task<IActionResult> GetRoomUsage(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (endDate <= startDate)
        {
            return BadRequest("End date must be later than start date.");
        }

        var report = await _reportService.GetRoomUsageAsync(startDate, endDate);

        return Ok(report);
    }
}