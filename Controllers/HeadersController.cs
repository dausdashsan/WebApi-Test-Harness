using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test/headers")]
    [Tags("Headers")]
    public class HeadersController : ControllerBase
    {
        [HttpGet("required")]
        public IActionResult GetRequiredHeaders([FromHeader(Name = "X-Api-Key")] string apiKey, [FromHeader(Name = "X-Request-ID")] string requestId)
        {
            return Ok(new { receivedHeaders = new { X_Api_Key = apiKey, X_Request_ID = requestId }, validated = true });
        }

        [HttpGet("optional")]
        public IActionResult GetOptionalHeaders([FromHeader(Name = "X-Custom-Header")] string? customHeader, [FromHeader(Name = "Accept-Language")] string acceptLanguage = "en-US")
        {
            return Ok(new { receivedHeaders = new { X_Custom_Header = customHeader, Accept_Language = acceptLanguage }, defaults = new { Accept_Language = "en-US" } });
        }

        [HttpGet("echo")]
        public IActionResult EchoHeaders()
        {
            var headers = new Dictionary<string, string>();
            foreach (var header in Request.Headers) headers[header.Key] = header.Value.ToString();
            return Ok(headers);
        }

        [HttpGet("custom")]
        public IActionResult GetCustomHeaders([FromHeader(Name = "X-Trace-ID")] string traceId, [FromHeader(Name = "X-User-ID")] string userId, [FromHeader(Name = "X-Tenant-ID")] string tenantId)
        {
            return Ok(new { traceId = traceId, userId = userId, tenantId = tenantId, headerVerification = new { X_Trace_ID = "forwarded", X_User_ID = "forwarded", X_Tenant_ID = "forwarded" } });
        }
    }
}
