using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test/headers")]
    [Tags("Headers")]
    public class HeadersController : ControllerBase
    {
        [HttpGet("required")]
        [SwaggerOperation(
            Summary = "Validate required request headers",
            Description = "Returns `400` if either `X-Api-Key` or `X-Request-ID` is missing from the request. Use this to verify that the gateway forwards custom headers to the backend and that the backend can enforce header requirements independently of the gateway."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetRequiredHeaders(
            [FromHeader(Name = "X-Api-Key"), SwaggerParameter("API key header. Must be present; value is echoed back in the response.", Required = true)] string apiKey,
            [FromHeader(Name = "X-Request-ID"), SwaggerParameter("Unique request correlation ID in UUID format. Must be present for tracing purposes.", Required = true)] string requestId)
        {
            return Ok(new { receivedHeaders = new { X_Api_Key = apiKey, X_Request_ID = requestId }, validated = true });
        }

        [HttpGet("optional")]
        [SwaggerOperation(
            Summary = "Handle optional request headers with defaults",
            Description = "Accepts optional custom headers and applies defaults when they are absent. Use this to verify the gateway does not inject or strip optional headers, and that backend defaults are applied correctly."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetOptionalHeaders(
            [FromHeader(Name = "X-Custom-Header"), SwaggerParameter("Arbitrary custom header value. Omit to test default behaviour.", Required = false)] string? customHeader,
            [FromHeader(Name = "Accept-Language"), SwaggerParameter("BCP 47 language tag for the preferred response language, e.g. `en-US` or `fr-FR`. Defaults to `en-US` if absent.", Required = false)] string acceptLanguage = "en-US")
        {
            return Ok(new { receivedHeaders = new { X_Custom_Header = customHeader, Accept_Language = acceptLanguage }, defaults = new { Accept_Language = "en-US" } });
        }

        [HttpGet("echo")]
        [SwaggerOperation(
            Summary = "Echo all received request headers",
            Description = "Returns every header received by the backend as a flat key/value map. Use this to verify exactly which headers the gateway forwards, modifies, or strips — including `Authorization`, `X-Forwarded-For`, trace headers, and any gateway-injected headers."
        )]
        [ProducesResponseType(200)]
        public IActionResult EchoHeaders()
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in Request.Headers) headers[header.Key] = header.Value.ToString();
            return Ok(headers);
        }

        [HttpGet("custom")]
        [SwaggerOperation(
            Summary = "Verify custom gateway forwarding headers",
            Description = "Checks that three specific tracing/identity headers — `X-Trace-ID`, `X-User-ID`, and `X-Tenant-ID` — are present and echoes their values. Use this to validate that the gateway correctly injects or forwards these headers when routing to the backend."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetCustomHeaders(
            [FromHeader(Name = "X-Trace-ID"), SwaggerParameter("Distributed trace ID injected by the gateway or caller, used to correlate logs across services.", Required = false)] string? traceId,
            [FromHeader(Name = "X-User-ID"), SwaggerParameter("Authenticated user ID injected by the gateway after token validation.", Required = false)] string? userId,
            [FromHeader(Name = "X-Tenant-ID"), SwaggerParameter("Tenant identifier for multi-tenant routing, injected by the gateway.", Required = false)] string? tenantId)
        {
            return Ok(new { traceId, userId, tenantId, headerVerification = new { X_Trace_ID = "forwarded", X_User_ID = "forwarded", X_Tenant_ID = "forwarded" } });
        }
    }
}
