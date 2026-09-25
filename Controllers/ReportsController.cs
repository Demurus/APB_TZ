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

    /// <summary>
    /// Returns total booking count and income for a selected period
    /// </summary>
    [HttpGet("income")]
    public async Task<IActionResult> GetIncome(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        if (endDate <= startDate)
        {
            return BadRequest("End date must be later than start date.");
        }

        var report = await _reportService.GetIncomeAsync(
            startDate,
            endDate,
            cancellationToken);
        return Ok(report);
    }
    
    /// <summary>
    /// Returns usage statistics for every conference room in a selected period
    /// </summary>
    [HttpGet("room-usage")]
    public async Task<IActionResult> GetRoomUsage(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        if (endDate <= startDate)
        {
            return BadRequest("End date must be later than start date.");
        }

        var report = await _reportService.GetRoomUsageAsync(
            startDate,
            endDate,
            cancellationToken);

        return Ok(report);
    }
}