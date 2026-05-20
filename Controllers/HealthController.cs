using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Tags("Health & Monitoring")]
    public class HealthController : ControllerBase
    {
        [HttpGet("health")]
        [SwaggerOperation(
            Summary = "Basic health check",
            Description = "Returns the current health status of the service. Use this as a liveness probe for load balancers and container orchestrators."
        )]
        [ProducesResponseType(200)]
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
        [SwaggerOperation(
            Summary = "Detailed dependency health check",
            Description = "Returns the health status of all downstream dependencies (database, cache, external APIs) with individual response times. Use as a readiness probe."
        )]
        [ProducesResponseType(200)]
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
        [SwaggerOperation(
            Summary = "System request metrics",
            Description = "Returns aggregated request metrics for the specified time period — total requests, success/failure counts, average response time, and 4xx/5xx error breakdown."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetMetrics(
            [FromQuery, SwaggerParameter("Time period for the metrics window. Allowed values: `1m` (1 minute), `5m` (5 minutes), `1h` (1 hour).", Required = false)] string period = "1m")
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
        [SwaggerOperation(
            Summary = "Application log entries",
            Description = "Returns recent application log entries with optional filtering by log level, date range, and result count. Useful for observability and debugging gateway traffic."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetLogs(
            [FromQuery, SwaggerParameter("Minimum log level to return. Allowed values: `DEBUG`, `INFO`, `WARN`, `ERROR`.", Required = false)] string level = "INFO",
            [FromQuery, SwaggerParameter("Maximum number of log entries to return (1–500).", Required = false)] int limit = 50,
            [FromQuery, SwaggerParameter("ISO 8601 start date/time for the log range filter, e.g. `2024-01-01T00:00:00Z`.", Required = false)] DateTime? startDate = null,
            [FromQuery, SwaggerParameter("ISO 8601 end date/time for the log range filter, e.g. `2024-12-31T23:59:59Z`.", Required = false)] DateTime? endDate = null)
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
