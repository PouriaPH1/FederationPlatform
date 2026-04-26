using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FederationPlatform.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                data = e.Value.Data
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };

        return report.Status == HealthStatus.Healthy
            ? Ok(response)
            : StatusCode(503, response);
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        // Readiness probe - check if app is ready to receive traffic
        return Ok(new { status = "Ready" });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        // Liveness probe - check if app is alive
        return Ok(new { status = "Live" });
    }
}
