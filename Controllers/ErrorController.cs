using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test/error")]
    [Tags("Error Handling")]
    public class ErrorController : ControllerBase
    {
        [HttpGet("400")]
        public IActionResult Get400() => BadRequest(new { statusCode = 400, message = "Bad Request", error = "INVALID_PARAMETER", details = new { field = "email", issue = "Invalid email format" }, traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("401")]
        public IActionResult Get401() => Unauthorized(new { statusCode = 401, message = "Unauthorized", error = "INVALID_CREDENTIALS", details = "Invalid username or password", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("403")]
        public IActionResult Get403() => StatusCode(403, new { statusCode = 403, message = "Forbidden", error = "INSUFFICIENT_PERMISSIONS", details = "User lacks 'admin' permission", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("404")]
        public IActionResult Get404() => NotFound(new { statusCode = 404, message = "Not Found", error = "RESOURCE_NOT_FOUND", details = "Resource with ID 12345 does not exist", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("500")]
        public IActionResult Get500() => StatusCode(500, new { statusCode = 500, message = "Internal Server Error", error = "INTERNAL_ERROR", details = "An unexpected error occurred", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("503")]
        public IActionResult Get503() {
            Response.Headers.RetryAfter = "60";
            return StatusCode(503, new { statusCode = 503, message = "Service Unavailable", error = "SERVICE_UNAVAILABLE", details = "Service temporarily unavailable", retryAfter = 60, traceId = "0HN8P4J5K2M1L0N" });
        }

        [HttpGet("timeout")]
        public async Task<IActionResult> GetTimeout([FromQuery] int delay = 30000)
        {
            await Task.Delay(delay);
            return StatusCode(408, new { statusCode = 408, message = "Request Timeout", error = "REQUEST_TIMEOUT", details = $"Request did not complete within {delay/1000} seconds", traceId = "0HN8P4J5K2M1L0N" });
        }
    }
}
