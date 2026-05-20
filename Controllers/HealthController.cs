using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Tags("Health & Monitoring")]
    public class HealthController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                uptime = 86400,
                version = "1.0.0"
            });
        }

        [HttpGet("health/detailed")]
        public IActionResult GetDetailedHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                services = new
                {
                    database = new { status = "healthy", responseTime = 5 },
                    cache = new { status = "healthy", responseTime = 2 },
                    externalApi = new { status = "healthy", responseTime = 150 }
                }
            });
        }

        [HttpGet("metrics")]
        public IActionResult GetMetrics([FromQuery] string period = "1m")
        {
            return Ok(new
            {
                period = period,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                requests = new
                {
                    total = 1500,
                    successful = 1485,
                    failed = 15,
                    average_response_time_ms = 45
                },
                errors = new
                {
                    _4xx = 10,
                    _5xx = 5
                }
            });
        }

        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery] string level = "INFO", [FromQuery] int limit = 50, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            return Ok(new
            {
                total = 50,
                logs = new[]
                {
                    new
                    {
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        level = level,
                        message = "Request received",
                        traceId = "0HN8P4J5K2M1L0N"
                    }
                }
            });
        }
    }
}
