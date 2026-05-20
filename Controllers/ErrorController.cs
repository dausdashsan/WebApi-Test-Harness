using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Trigger a 400 Bad Request response",
            Description = "Always returns `400 Bad Request` with a structured error body. Use this to verify the gateway correctly propagates 4xx error responses from the backend — including the status code, body, and headers — without swallowing or transforming them."
        )]
        [ProducesResponseType(400)]
        public IActionResult Get400() => BadRequest(new { statusCode = 400, message = "Bad Request", error = "INVALID_PARAMETER", details = new { field = "email", issue = "Invalid email format" }, traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("401")]
        [SwaggerOperation(
            Summary = "Trigger a 401 Unauthorized response",
            Description = "Always returns `401 Unauthorized`. Use this to verify the gateway does not intercept or alter 401 responses from the backend, and that `WWW-Authenticate` or error body details are forwarded correctly."
        )]
        [ProducesResponseType(401)]
        public IActionResult Get401() => Unauthorized(new { statusCode = 401, message = "Unauthorized", error = "INVALID_CREDENTIALS", details = "Invalid username or password", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("403")]
        [SwaggerOperation(
            Summary = "Trigger a 403 Forbidden response",
            Description = "Always returns `403 Forbidden`. Use this to verify the gateway distinguishes between authentication failures (401) and authorization failures (403), and forwards both correctly without substituting its own error response."
        )]
        [ProducesResponseType(403)]
        public IActionResult Get403() => StatusCode(403, new { statusCode = 403, message = "Forbidden", error = "INSUFFICIENT_PERMISSIONS", details = "User lacks 'admin' permission", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("404")]
        [SwaggerOperation(
            Summary = "Trigger a 404 Not Found response",
            Description = "Always returns `404 Not Found`. Use this to verify the gateway forwards backend 404 responses without replacing them with its own 404 (e.g. for an unmatched route)."
        )]
        [ProducesResponseType(404)]
        public IActionResult Get404() => NotFound(new { statusCode = 404, message = "Not Found", error = "RESOURCE_NOT_FOUND", details = "Resource with ID 12345 does not exist", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("500")]
        [SwaggerOperation(
            Summary = "Trigger a 500 Internal Server Error response",
            Description = "Always returns `500 Internal Server Error`. Use this to verify the gateway forwards backend 5xx errors without masking them as a gateway error (502/503) and that the backend error body is preserved."
        )]
        [ProducesResponseType(500)]
        public IActionResult Get500() => StatusCode(500, new { statusCode = 500, message = "Internal Server Error", error = "INTERNAL_ERROR", details = "An unexpected error occurred", traceId = "0HN8P4J5K2M1L0N" });

        [HttpGet("503")]
        [SwaggerOperation(
            Summary = "Trigger a 503 Service Unavailable response",
            Description = "Always returns `503 Service Unavailable` with a `Retry-After: 60` header. Use this to verify the gateway forwards backend 503 responses and does not suppress the `Retry-After` header that clients need for back-off logic."
        )]
        [ProducesResponseType(503)]
        public IActionResult Get503()
        {
            Response.Headers.RetryAfter = "60";
            return StatusCode(503, new { statusCode = 503, message = "Service Unavailable", error = "SERVICE_UNAVAILABLE", details = "Service temporarily unavailable", retryAfter = 60, traceId = "0HN8P4J5K2M1L0N" });
        }

        [HttpGet("timeout")]
        [SwaggerOperation(
            Summary = "Simulate a slow/timeout response",
            Description = "Delays the response by `delay` milliseconds before returning `408 Request Timeout`. Use this to test gateway timeout settings — if the gateway timeout is shorter than `delay`, the gateway should return its own timeout error; if longer, the backend's 408 should be forwarded."
        )]
        [ProducesResponseType(408)]
        public async Task<IActionResult> GetTimeout(
            [FromQuery, SwaggerParameter("How long to wait before responding, in milliseconds. Default is `5000` (5 seconds). Set above the gateway's timeout to trigger a gateway-level timeout.", Required = false)] int delay = 5000)
        {
            await Task.Delay(delay);
            return StatusCode(408, new { statusCode = 408, message = "Request Timeout", error = "REQUEST_TIMEOUT", details = $"Request did not complete within {delay / 1000} seconds", traceId = "0HN8P4J5K2M1L0N" });
        }
    }
}
