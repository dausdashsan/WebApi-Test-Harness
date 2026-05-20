using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApiTestHarness.Controllers
{
    public class TextRequest { public string? Text { get; set; } }

    [ApiController]
    public class AdvancedController : ControllerBase
    {
        [HttpPost("transform/uppercase")]
        [Tags("Advanced Features")]
        public IActionResult Uppercase([FromBody] TextRequest request) => Ok(new { original = request.Text, result = request.Text?.ToUpper(), transformation = "uppercase" });

        [HttpPost("transform/encrypt")]
        [Tags("Advanced Features")]
        public IActionResult Encrypt([FromBody] dynamic body) => Ok(new { original = "secret message", encrypted = "u2FsdGVkX1...", algorithm = "AES-256", encoding = "base64" });

        [HttpPost("transform/decompress")]
        [Tags("Advanced Features")]
        public IActionResult Decompress([FromBody] byte[] data) => Ok(new { decompressed = "original content...", originalSize = 1024, compressedSize = 512, algorithm = "gzip" });

        [HttpGet("test/compression")]
        [Tags("Advanced Features")]
        public IActionResult Compression([FromQuery] string algorithm = "gzip") => Ok(new { message = $"Response compressed with {algorithm}", originalSize = 1024, compressedSize = 256, compressionRatio = 0.25 });

        [HttpGet("test/metadata")]
        [Tags("Advanced Features")]
        public IActionResult Metadata() => Ok(new { request = new { method = "GET", path = "/test/metadata", timestamp = DateTime.UtcNow }, response = new { statusCode = 200, processingTime = 45, timestamp = DateTime.UtcNow } });

        [HttpGet("test/tracing")]
        [Tags("Advanced Features")]
        public IActionResult Tracing() => Ok(new { traceId = "0HN8P4J5K2M1L0N", spans = new[] { new { spanId = "span-1", name = "http-request", startTime = DateTime.UtcNow, duration = 45 } } });

        [HttpGet("test/rate-limited")]
        [HttpGet("test/rate-limited/high")]
        [HttpGet("test/rate-limited/low")]
        [Tags("Advanced Features")]
        public IActionResult RateLimited()
        {
            Response.Headers["X-RateLimit-Limit"] = "10";
            Response.Headers["X-RateLimit-Remaining"] = "5";
            Response.Headers["X-RateLimit-Reset"] = "1705318800";
            return Ok(new { message = "Request successful", rateLimit = new { limit = 10, remaining = 5, reset = "2024-01-15T11:30:00Z" } });
        }

        [HttpPost("webhooks/register")]
        [Tags("Advanced Features")]
        public IActionResult RegisterWebhook([FromBody] dynamic body) => Created("", new { webhookId = "webhook-uuid", url = "https://example.com/webhook", active = true, createdAt = DateTime.UtcNow });

        [HttpPost("webhooks/{webhookId}/test")]
        [Tags("Advanced Features")]
        public IActionResult TestWebhook(string webhookId) => Ok(new { webhookId, testId = Guid.NewGuid().ToString(), status = "success", responseStatus = 200 });

        [HttpGet("api/v1/test/simple")]
        [Tags("Advanced Features")]
        public IActionResult V1Simple() => Ok(new { version = "1.0.0", message = "API v1 endpoint", deprecation = "v1 will be deprecated on 2024-12-31" });

        [HttpGet("api/v2/test/simple")]
        [Tags("Advanced Features")]
        public IActionResult V2Simple() => Ok(new { version = "2.0.0", message = "API v2 endpoint" });

        [HttpPost("test/idempotent")]
        [Tags("Advanced Features")]
        public IActionResult Idempotent([FromHeader(Name = "Idempotency-Key")] string key) => Ok(new { id = Guid.NewGuid().ToString(), idempotencyKey = key, status = "created", createdAt = DateTime.UtcNow });
    }
}
