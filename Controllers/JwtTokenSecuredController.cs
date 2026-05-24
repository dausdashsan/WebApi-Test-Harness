using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("secure/jwt")]
    [Tags("Z JwtToken")]
    public class JwtTokenSecuredController : ControllerBase
    {
        private const string JwtSecret = "your_jwt_secret_key_at_least_32_characters_long_for_security";

        private static readonly Dictionary<string, JwtTokenInfo> ValidJwtTokens = new()
        {
            { "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidXNlcm5hbWUiOiJhZG1pbiIsInJvbGUiOiJhZG1pbiIsImlhdCI6MTcwNDAwMDAwMCwiZXhwIjoxNzA1MjA5NjAwfQ.admin_jwt_signature",
                new JwtTokenInfo { Username = "admin", UserId = "user_001", Role = "admin", Permissions = new[] { "read", "write", "delete" }, IssuedAt = DateTime.UtcNow.AddHours(-1), ExpiresAt = DateTime.UtcNow.AddHours(23) } },
            { "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIyIiwidXNlcm5hbWUiOiJ1c2VyIiwicm9sZSI6InVzZXIiLCJpYXQiOjE3MDQwMDAwMDAsImV4cCI6MTcwNDAwMzYwMH0.user_jwt_signature",
                new JwtTokenInfo { Username = "user", UserId = "user_002", Role = "user", Permissions = new[] { "read", "write" }, IssuedAt = DateTime.UtcNow.AddHours(-2), ExpiresAt = DateTime.UtcNow.AddHours(22) } },
            { "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwidXNlcm5hbWUiOiJndWVzdCIsInJvbGUiOiJndWVzdCIsImlhdCI6MTcwNDAwMDAwMCwiZXhwIjoxNzA0MDAwMzYwfQ.guest_jwt_signature",
                new JwtTokenInfo { Username = "guest", UserId = "user_003", Role = "guest", Permissions = new[] { "read" }, IssuedAt = DateTime.UtcNow.AddHours(-3), ExpiresAt = DateTime.UtcNow.AddHours(21) } }
        };

        [HttpGet("protected")]
        [SwaggerOperation(
            Summary = "Protected endpoint - JWT Token",
            Description = "Requires JWT Bearer Token. Send `Authorization: Bearer {jwt_token}` header. Use `/secure/jwt/generate` to generate a valid JWT token."
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

            if (!ValidJwtTokens.TryGetValue(token, out var jwtInfo))
            {
                return Unauthorized(new { error = "Invalid or expired JWT token" });
            }

            if (DateTime.UtcNow > jwtInfo.ExpiresAt)
            {
                return Unauthorized(new { error = "JWT token has expired" });
            }

            return Ok(new
            {
                success = true,
                message = "JWT token validated successfully",
                user = jwtInfo.Username,
                resource = new
                {
                    id = Guid.NewGuid().ToString(),
                    title = "JWT Protected Resource",
                    content = "This data is protected by JWT authentication",
                    data = new
                    {
                        userId = jwtInfo.UserId,
                        role = jwtInfo.Role,
                        permissions = jwtInfo.Permissions,
                        lastAccessed = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                },
                accessedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("decode")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Decode JWT token information",
            Description = "Returns information about JWT Token authentication. No authentication required. Use the /generate endpoint to create a JWT token for testing."
        )]
        [ProducesResponseType(200)]
        public IActionResult DecodeToken()
        {
            return Ok(new
            {
                authType = "JWT Bearer Token",
                description = "JWT (JSON Web Token) authentication using signed tokens",
                validTokens = new[]
                {
                    new { username = "admin", role = "admin", permissions = new[] { "read", "write", "delete" } },
                    new { username = "user", role = "user", permissions = new[] { "read", "write" } },
                    new { username = "guest", role = "guest", permissions = new[] { "read" } }
                },
                headerFormat = "Authorization: Bearer {jwt_token}",
                exampleHeader = "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                generateEndpoint = "GET /secure/jwt/generate?username=admin",
                protectedEndpoint = "GET /secure/jwt/protected",
                notes = "Use the /generate endpoint to create a valid JWT token for testing"
            });
        }

        [HttpGet("generate")]
        [SwaggerOperation(
            Summary = "Generate a JWT token",
            Description = "Generates a valid JWT token for testing. Pass `?username=admin` to generate admin token, `?username=user` for user token, or `?username=guest` for guest token."
        )]
        [ProducesResponseType(200)]
        public IActionResult GenerateToken(
            [FromQuery, SwaggerParameter("Username to generate token for (admin, user, guest)", Required = false)] string username = "user")
        {
            if (!ValidJwtTokens.ContainsValue(new JwtTokenInfo { Username = username }))
            {
                if (username != "admin" && username != "user" && username != "guest")
                {
                    return BadRequest(new { error = "Invalid username. Valid values: admin, user, guest" });
                }
            }

            var jwtInfo = new JwtTokenInfo
            {
                Username = username,
                UserId = username == "admin" ? "user_001" : username == "user" ? "user_002" : "user_003",
                Role = username == "admin" ? "admin" : username == "user" ? "user" : "guest",
                Permissions = username == "admin" ? new[] { "read", "write", "delete" } : username == "user" ? new[] { "read", "write" } : new[] { "read" },
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            var token = GenerateJwtToken(jwtInfo);

            return Ok(new
            {
                token = token,
                username = jwtInfo.Username,
                userId = jwtInfo.UserId,
                role = jwtInfo.Role,
                permissions = jwtInfo.Permissions,
                expiresIn = 86400,
                expiresAt = jwtInfo.ExpiresAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                tokenType = "Bearer"
            });
        }

        private string GenerateJwtToken(JwtTokenInfo info)
        {
            var key = Encoding.ASCII.GetBytes(JwtSecret);
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{\"alg\":\"HS256\",\"typ\":\"JWT\"}}")).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{\"sub\":\"{info.UserId}\",\"username\":\"{info.Username}\",\"role\":\"{info.Role}\",\"iat\":{(long)(info.IssuedAt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds},\"exp\":{(long)(info.ExpiresAt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}}}")).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            return $"{header}.{payload}.signature";
        }

        private class JwtTokenInfo
        {
            public string Username { get; set; }
            public string UserId { get; set; }
            public string Role { get; set; }
            public string[] Permissions { get; set; }
            public DateTime IssuedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}
