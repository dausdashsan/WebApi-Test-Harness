using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("secure/bearer-token")]
    [Tags("Z BearerToken")]
    public class BearerTokenSecuredController : ControllerBase
    {
        private static readonly Dictionary<string, BearerTokenInfo> ValidTokens = new()
        {
            { "bearer_token_admin_12345", new BearerTokenInfo { Username = "admin", UserId = "user_001", Permissions = new[] { "read", "write", "delete" }, Role = "admin", IssuedAt = DateTime.UtcNow.AddHours(-1) } },
            { "bearer_token_user_67890", new BearerTokenInfo { Username = "user", UserId = "user_002", Permissions = new[] { "read", "write" }, Role = "user", IssuedAt = DateTime.UtcNow.AddHours(-2) } },
            { "bearer_token_guest_11111", new BearerTokenInfo { Username = "guest", UserId = "user_003", Permissions = new[] { "read" }, Role = "guest", IssuedAt = DateTime.UtcNow.AddHours(-3) } }
        };

        [HttpGet("protected")]
        [SwaggerOperation(
            Summary = "Protected endpoint - Bearer Token",
            Description = "Requires Bearer Token authentication. Send `Authorization: Bearer {token}` header. Valid tokens: bearer_token_admin_12345, bearer_token_user_67890, bearer_token_guest_11111"
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetProtectedResource()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "Missing or invalid Authorization header" });
            }

            var token = authHeader.Substring("Bearer ".Length);

            if (!ValidTokens.TryGetValue(token, out var tokenInfo))
            {
                return Unauthorized(new { error = "Invalid or expired bearer token" });
            }

            return Ok(new
            {
                success = true,
                message = "Access granted",
                user = tokenInfo.Username,
                resource = new
                {
                    id = Guid.NewGuid().ToString(),
                    title = "User Dashboard Data",
                    content = "This is protected data accessible with a valid Bearer Token",
                    userStats = new
                    {
                        loginCount = 42,
                        lastLogin = DateTime.UtcNow.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        totalRequests = 1250
                    },
                    accessedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            });
        }

        [HttpGet("token-info")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get Bearer Token information",
            Description = "Returns information about Bearer Token authentication. No authentication required. Valid tokens: bearer_token_admin_12345, bearer_token_user_67890, bearer_token_guest_11111"
        )]
        [ProducesResponseType(200)]
        public IActionResult GetTokenInfo()
        {
            return Ok(new
            {
                authType = "Bearer Token",
                description = "Bearer Token authentication using a simple token string",
                validTokens = new[]
                {
                    new { token = "bearer_token_admin_12345", username = "admin", role = "admin", permissions = new[] { "read", "write", "delete" } },
                    new { token = "bearer_token_user_67890", username = "user", role = "user", permissions = new[] { "read", "write" } },
                    new { token = "bearer_token_guest_11111", username = "guest", role = "guest", permissions = new[] { "read" } }
                },
                headerFormat = "Authorization: Bearer {token}",
                exampleHeader = "Authorization: Bearer bearer_token_admin_12345",
                protectedEndpoint = "GET /secure/bearer-token/protected"
            });
        }

        private class BearerTokenInfo
        {
            public string Username { get; set; }
            public string UserId { get; set; }
            public string[] Permissions { get; set; }
            public string Role { get; set; }
            public DateTime IssuedAt { get; set; }
        }
    }
}
