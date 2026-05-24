using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("secure/api-key")]
    [Tags("Z ApiKey")]
    public class ApiKeySecurityController : ControllerBase
    {
        private static readonly Dictionary<string, ApiKeyInfo> ValidApiKeys = new()
        {
            { "sk_live_admin_key_abc123def456", new ApiKeyInfo { KeyId = "key_001", Name = "Admin API Key", Username = "admin", UserId = "user_001", Permissions = new[] { "read", "write", "delete" }, Role = "admin", CreatedAt = DateTime.UtcNow.AddMonths(-1), ExpiresAt = DateTime.UtcNow.AddYears(1) } },
            { "sk_live_user_key_xyz789uvw012", new ApiKeyInfo { KeyId = "key_002", Name = "User API Key", Username = "user", UserId = "user_002", Permissions = new[] { "read", "write" }, Role = "user", CreatedAt = DateTime.UtcNow.AddMonths(-2), ExpiresAt = DateTime.UtcNow.AddYears(1) } },
            { "sk_test_limited_key_pqr345stu678", new ApiKeyInfo { KeyId = "key_003", Name = "Test API Key", Username = "test", UserId = "user_003", Permissions = new[] { "read" }, Role = "guest", CreatedAt = DateTime.UtcNow.AddMonths(-3), ExpiresAt = DateTime.UtcNow.AddMonths(6) } }
        };

        [HttpGet("protected")]
        [SwaggerOperation(
            Summary = "Protected endpoint - API Key",
            Description = "Requires API Key authentication. Send `X-API-Key: {api_key}` header. Valid keys: sk_live_admin_key_abc123def456, sk_live_user_key_xyz789uvw012, sk_test_limited_key_pqr345stu678"
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetProtectedResource()
        {
            var apiKey = Request.Headers["X-API-Key"].ToString();

            if (string.IsNullOrEmpty(apiKey))
            {
                return Unauthorized(new { error = "Missing X-API-Key header" });
            }

            if (!ValidApiKeys.TryGetValue(apiKey, out var keyInfo))
            {
                return Unauthorized(new { error = "Invalid or revoked API key" });
            }

            if (DateTime.UtcNow > keyInfo.ExpiresAt)
            {
                return Unauthorized(new { error = "API key has expired" });
            }

            return Ok(new
            {
                success = true,
                message = "API Key authenticated successfully",
                user = keyInfo.Username,
                resource = new
                {
                    id = Guid.NewGuid().ToString(),
                    title = "API Key Protected Data",
                    content = "This resource is protected by API Key authentication",
                    apiUsage = new
                    {
                        requestsThisMonth = 1523,
                        requestsRemaining = 8477,
                        quotaLimit = 10000
                    },
                    accessedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            });
        }

        [HttpGet("key-info")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get API Key information",
            Description = "Returns information about API Key authentication. No authentication required. Valid keys: sk_live_admin_key_abc123def456, sk_live_user_key_xyz789uvw012, sk_test_limited_key_pqr345stu678"
        )]
        [ProducesResponseType(200)]
        public IActionResult GetKeyInfo()
        {
            return Ok(new
            {
                authType = "API Key",
                description = "API Key authentication using X-API-Key header",
                validKeys = new[]
                {
                    new { key = "sk_live_admin_key_abc123def456", username = "admin", role = "admin", permissions = new[] { "read", "write", "delete" } },
                    new { key = "sk_live_user_key_xyz789uvw012", username = "user", role = "user", permissions = new[] { "read", "write" } },
                    new { key = "sk_test_limited_key_pqr345stu678", username = "test", role = "guest", permissions = new[] { "read" } }
                },
                headerFormat = "X-API-Key: {api_key}",
                exampleHeader = "X-API-Key: sk_live_admin_key_abc123def456",
                protectedEndpoint = "GET /secure/api-key/protected"
            });
        }

        private class ApiKeyInfo
        {
            public string KeyId { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string UserId { get; set; }
            public string[] Permissions { get; set; }
            public string Role { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}
