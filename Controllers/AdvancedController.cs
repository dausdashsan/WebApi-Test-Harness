using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace WebApiTestHarness.Controllers
{
    public class TextRequest { public string? Text { get; set; } }

    [ApiController]
    public class AdvancedController : ControllerBase
    {
        [HttpPost("transform/uppercase")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Transform text to uppercase",
            Description = "Converts the `text` field in the request body to uppercase and returns both the original and transformed values. Use this to test gateway response body transformation rules."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Uppercase(
            [FromBody, SwaggerRequestBody("Object containing the text to transform.", Required = true)] TextRequest request)
            => Ok(new { original = request.Text, result = request.Text?.ToUpper(), transformation = "uppercase" });

        [HttpPost("transform/encrypt")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Encrypt data (mock AES-256)",
            Description = "Accepts a payload and returns a mock AES-256 Base64-encrypted representation. The encryption is simulated — use this to test gateway handling of encrypted payloads, not for real security."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Encrypt(
            [FromBody, SwaggerRequestBody("Object containing the `data` string to encrypt, e.g. `{ \"data\": \"secret message\" }`.", Required = true)] dynamic body)
            => Ok(new { original = "secret message", encrypted = "u2FsdGVkX1...", algorithm = "AES-256", encoding = "base64" });

        [HttpPost("transform/decompress")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Decompress a gzip/deflate payload (mock)",
            Description = "Accepts a compressed binary body and returns the decompressed content along with size metrics. The decompression is simulated. Use this to test gateway `Content-Encoding` handling."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Decompress(
            [FromBody, SwaggerRequestBody("Compressed binary data (gzip or deflate).", Required = true)] byte[] data)
            => Ok(new { decompressed = "original content...", originalSize = 1024, compressedSize = 512, algorithm = "gzip" });

        [HttpGet("test/compression")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Request a compressed response",
            Description = "Returns a JSON response compressed with the specified algorithm. The `Content-Encoding` response header reflects the applied compression. Use this to verify the gateway correctly decompresses or transparently forwards compressed responses."
        )]
        [ProducesResponseType(200)]
        public IActionResult Compression(
            [FromQuery, SwaggerParameter("Compression algorithm for the response body. Allowed values: `gzip`, `deflate`, `br` (Brotli).", Required = false)] string algorithm = "gzip")
            => Ok(new { message = $"Response compressed with {algorithm}", originalSize = 1024, compressedSize = 256, compressionRatio = 0.25 });

        [HttpGet("test/metadata")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Get request and response metadata",
            Description = "Returns a snapshot of the incoming request metadata (method, path, headers, timestamp) alongside response metadata (status, processing time). Use this to inspect what the backend sees after the gateway transforms the request."
        )]
        [ProducesResponseType(200)]
        public IActionResult Metadata()
            => Ok(new { request = new { method = "GET", path = "/test/metadata", timestamp = DateTime.UtcNow }, response = new { statusCode = 200, processingTime = 45, timestamp = DateTime.UtcNow } });

        [HttpGet("test/tracing")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Get distributed trace information",
            Description = "Returns the trace ID and span details for the current request. Use this to verify that the gateway injects or forwards `X-Trace-ID` / `traceparent` headers so distributed traces can be correlated across the gateway and backend."
        )]
        [ProducesResponseType(200)]
        public IActionResult Tracing()
            => Ok(new { traceId = "0HN8P4J5K2M1L0N", spans = new[] { new { spanId = "span-1", name = "http-request", startTime = DateTime.UtcNow, duration = 45 } } });

        [HttpGet("test/rate-limited")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Rate-limited endpoint (10 req/min)",
            Description = "Returns rate limit headers: `X-RateLimit-Limit: 10`, `X-RateLimit-Remaining`, `X-RateLimit-Reset`. Use this to verify the gateway reads and respects backend rate-limit headers, or to test that the gateway's own rate limiter returns these headers on `429` responses."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(429)]
        public IActionResult RateLimited()
        {
            Response.Headers["X-RateLimit-Limit"] = "10";
            Response.Headers["X-RateLimit-Remaining"] = "5";
            Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString();
            return Ok(new { message = "Request successful", rateLimit = new { limit = 10, remaining = 5, reset = DateTime.UtcNow.AddMinutes(1) } });
        }

        [HttpGet("test/rate-limited/high")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Rate-limited endpoint — high limit (100 req/min)",
            Description = "Same as `GET /test/rate-limited` but with a higher limit of 100 requests per minute. Use this alongside the low-limit endpoint to test the gateway's ability to enforce different rate limits per route."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(429)]
        public IActionResult RateLimitedHigh()
        {
            Response.Headers["X-RateLimit-Limit"] = "100";
            Response.Headers["X-RateLimit-Remaining"] = "95";
            Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString();
            return Ok(new { message = "Request successful", rateLimit = new { limit = 100, remaining = 95, reset = DateTime.UtcNow.AddMinutes(1) } });
        }

        [HttpGet("test/rate-limited/low")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Rate-limited endpoint — low limit (1 req/min)",
            Description = "Same as `GET /test/rate-limited` but with a strict limit of 1 request per minute. Use this to easily trigger `429 Too Many Requests` and verify the gateway forwards or generates the correct retry-after response."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(429)]
        public IActionResult RateLimitedLow()
        {
            Response.Headers["X-RateLimit-Limit"] = "1";
            Response.Headers["X-RateLimit-Remaining"] = "0";
            Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString();
            return Ok(new { message = "Request successful", rateLimit = new { limit = 1, remaining = 0, reset = DateTime.UtcNow.AddMinutes(1) } });
        }

        [HttpPost("webhooks/register")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Register a webhook endpoint",
            Description = "Registers a URL to receive webhook event notifications. Returns a shared secret (`whsec_...`) that can be used to verify the HMAC signature on incoming webhook payloads. Use `POST /webhooks/{webhookId}/test` to send a test delivery."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult RegisterWebhook(
            [FromBody, SwaggerRequestBody("Webhook registration details — target URL and list of event types to subscribe to, e.g. `[\"file.uploaded\", \"user.created\"]`.", Required = true)] dynamic body)
            => Created("", new { webhookId = Guid.NewGuid().ToString(), url = "https://example.com/webhook", events = new[] { "file.uploaded", "user.created" }, secret = "whsec_test1234567890", active = true, createdAt = DateTime.UtcNow });

        [HttpPost("webhooks/{webhookId}/test")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Send a test webhook delivery",
            Description = "Triggers a test webhook delivery to the URL registered for `webhookId`. Returns the HTTP status received from the target URL and the round-trip latency. Use this to verify the webhook target is reachable and processing payloads correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult TestWebhook(
            [SwaggerParameter("The webhook ID returned when the webhook was registered.", Required = true)] string webhookId)
            => Ok(new { webhookId, testId = Guid.NewGuid().ToString(), status = "success", responseStatus = 200, responseTime = 250 });

        [HttpGet("api/v1/test/simple")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "API v1 — simple endpoint (deprecated)",
            Description = "Version 1 of the simple test endpoint. Returns a deprecation notice alongside the response. Use this with `GET /api/v2/test/simple` to test gateway version routing — e.g. routing `/api/v1/...` and `/api/v2/...` to different backend services."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(410)]
        public IActionResult V1Simple()
            => Ok(new { version = "1.0.0", message = "API v1 endpoint", deprecation = "v1 will be deprecated on 2024-12-31" });

        [HttpGet("api/v2/test/simple")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "API v2 — simple endpoint (current)",
            Description = "Version 2 of the simple test endpoint. This is the current supported version. Use this alongside `GET /api/v1/test/simple` to test the gateway's URL-path-based version routing."
        )]
        [ProducesResponseType(200)]
        public IActionResult V2Simple()
            => Ok(new { version = "2.0.0", message = "API v2 endpoint" });

        [HttpPost("test/idempotent")]
        [Tags("Advanced Features")]
        [SwaggerOperation(
            Summary = "Idempotent operation with deduplication key",
            Description = "Accepts an `Idempotency-Key` header (UUID). The first request with a given key creates the resource and returns `status: created`. Subsequent requests with the same key return the same result with `status: duplicate` — the operation is not re-executed. Use this to test gateway idempotency passthrough."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(409)]
        public IActionResult Idempotent(
            [FromHeader(Name = "Idempotency-Key"), SwaggerParameter("A client-generated UUID that uniquely identifies this request. Must be a valid UUID v4, e.g. `550e8400-e29b-41d4-a716-446655440000`.", Required = true)] string key)
            => Ok(new { id = Guid.NewGuid().ToString(), idempotencyKey = key, status = "created", createdAt = DateTime.UtcNow });
    }
}
