using Microsoft.AspNetCore.Mvc;
using ConferenceBookingApi.DTOs;
using ConferenceBookingApi.Services.Interfaces;

namespace ConferenceBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    // Зведений звіт аналітики
    [HttpGet("summary")]
    [ProducesResponseType(typeof(AnalyticsSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaryReport()
    {
        var summary = await _analyticsService.GetSummaryReportAsync();
        return Ok(summary);
    }
}