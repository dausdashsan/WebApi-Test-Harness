using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("secure/basic-auth")]
    [Tags("Z BasicAuth")]
    public class BasicAuthSecuredController : ControllerBase
    {
        private static readonly string[] ValidUsers = { "admin", "user", "test" };
        private static readonly string[] ValidPasswords = { "password123", "user@123", "test@123" };
        private static readonly Dictionary<string, string> UserCredentials = new()
        {
            { "admin", "password123" },
            { "user", "user@123" },
            { "test", "test@123" }
        };

        [HttpGet("protected")]
        [SwaggerOperation(
            Summary = "Protected endpoint - Basic Auth",
            Description = "Requires Basic Authentication. Send `Authorization: Basic base64(username:password)` header. Valid credentials: admin/password123, user/user@123, test/test@123"
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetProtectedResource()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                return Unauthorized(new { error = "Missing or invalid Authorization header" });
            }

            try
            {
                var encodedCredentials = authHeader.Substring("Basic ".Length);
                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                var parts = decodedCredentials.Split(':');

                if (parts.Length != 2)
                {
                    return Unauthorized(new { error = "Invalid credential format" });
                }

                var username = parts[0];
                var password = parts[1];

                if (UserCredentials.TryGetValue(username, out var correctPassword) && correctPassword == password)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Access granted",
                        user = username,
                        resource = new
                        {
                            id = Guid.NewGuid().ToString(),
                            title = "Confidential Document",
                            content = "This is protected data only accessible with valid Basic Auth credentials",
                            accessedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                        }
                    });
                }

                return Unauthorized(new { error = "Invalid username or password" });
            }
            catch
            {
                return Unauthorized(new { error = "Failed to parse Basic Auth credentials" });
            }
        }

        [HttpGet("credentials")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get Basic Auth credential info",
            Description = "Returns information about Basic Auth credentials. No authentication required. Valid credentials: admin/password123, user/user@123, test/test@123"
        )]
        [ProducesResponseType(200)]
        public IActionResult GetCredentialInfo()
        {
            return Ok(new
            {
                authType = "Basic Auth",
                description = "HTTP Basic Authentication using Base64 encoded username:password",
                validCredentials = new[]
                {
                    new { username = "admin", password = "password123", role = "admin", permissions = new[] { "read", "write", "delete" } },
                    new { username = "user", password = "user@123", role = "user", permissions = new[] { "read", "write" } },
                    new { username = "test", password = "test@123", role = "guest", permissions = new[] { "read" } }
                },
                headerFormat = "Authorization: Basic base64(username:password)",
                exampleHeader = "Authorization: Basic YWRtaW46cGFzc3dvcmQxMjM=",
                protectedEndpoint = "GET /secure/basic-auth/protected"
            });
        }
    }
}
