using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("secure/oauth")]
    [Tags("Z OAuth20")]
    public class OAuth2SecuredController : ControllerBase
    {
        private static readonly Dictionary<string, OAuthAccessToken> AccessTokens = new();

        private static readonly Dictionary<string, OAuthUser> OAuthUsers = new()
        {
            { "admin", new OAuthUser { Username = "admin", UserId = "user_001", Email = "admin@example.com", Permissions = new[] { "read", "write", "delete" }, Role = "admin" } },
            { "user", new OAuthUser { Username = "user", UserId = "user_002", Email = "user@example.com", Permissions = new[] { "read", "write" }, Role = "user" } },
            { "guest", new OAuthUser { Username = "guest", UserId = "user_003", Email = "guest@example.com", Permissions = new[] { "read" }, Role = "guest" } }
        };

        private const string ClientId = "client_123456";
        private const string ClientSecret = "client_secret_abcdefghij1234567890";

    
        [HttpPost("token")]
        [SwaggerOperation(
            Summary = "OAuth 2.0 Token endpoint (Client Credentials)",
            Description = "Issues an access token using the Client Credentials grant type. Requires client_id, client_secret, grant_type=client_credentials, and scope."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Token(
            [FromForm, SwaggerParameter("OAuth 2.0 client identifier", Required = true)] string client_id,
            [FromForm, SwaggerParameter("OAuth 2.0 client secret", Required = true)] string client_secret,
            [FromForm, SwaggerParameter("Must be 'client_credentials'", Required = true)] string grant_type,
            [FromForm, SwaggerParameter("Space-separated list of scopes (e.g., 'read write delete')", Required = true)] string scope)
        {
            if (client_id != ClientId || client_secret != ClientSecret || grant_type != "client_credentials")
            {
                return BadRequest(new { error = "invalid_client", error_description = "Invalid client_id, client_secret, or grant_type" });
            }

            if (string.IsNullOrWhiteSpace(scope))
            {
                return BadRequest(new { error = "invalid_scope", error_description = "scope parameter is required" });
            }

            var accessToken = "access_token_" + Guid.NewGuid().ToString().Substring(0, 32);
            var expiresIn = 3600;

            AccessTokens[accessToken] = new OAuthAccessToken
            {
                Token = accessToken,
                RefreshToken = null,
                Username = "client_credentials",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn)
            };

            return Ok(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = expiresIn,
                scope = scope
            });
        }

        [HttpGet("protected")]
        [SwaggerOperation(
            Summary = "Protected endpoint - OAuth 2.0",
            Description = "Requires valid OAuth 2.0 access token. Send `Authorization: Bearer {access_token}` header."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult GetProtectedResource()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "invalid_token", error_description = "Missing or invalid Authorization header" });
            }

            var token = authHeader.Substring("Bearer ".Length);

            if (!AccessTokens.TryGetValue(token, out var accessToken))
            {
                return Unauthorized(new { error = "invalid_token", error_description = "Access token not found or has been revoked" });
            }

            if (DateTime.UtcNow > accessToken.ExpiresAt)
            {
                AccessTokens.Remove(token);
                return Unauthorized(new { error = "token_expired", error_description = "Access token has expired" });
            }

            if (!OAuthUsers.TryGetValue(accessToken.Username, out var user))
            {
                return Unauthorized(new { error = "invalid_token", error_description = "User not found" });
            }

            return Ok(new
            {
                success = true,
                message = "OAuth 2.0 token validated successfully",
                user = user.Username,
                resource = new
                {
                    id = Guid.NewGuid().ToString(),
                    title = "OAuth Protected Resource",
                    content = "This data is protected by OAuth 2.0 authentication",
                    userData = new
                    {
                        username = user.Username,
                        email = user.Email,
                        role = user.Role,
                        permissions = user.Permissions
                    }
                },
                accessedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("userinfo")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get OAuth 2.0 information",
            Description = "Returns information about OAuth 2.0 Client Credentials Flow. No authentication required. Client ID: client_123456, Client Secret: client_secret_abcdefghij1234567890"
        )]
        [ProducesResponseType(200)]
        public IActionResult GetUserInfo()
        {
            return Ok(new
            {
                authType = "OAuth 2.0",
                description = "OAuth 2.0 Client Credentials Flow",
                clientId = "client_123456",
                clientSecret = "client_secret_abcdefghij1234567890",
                flow = new
                {
                    step1 = "POST /secure/oauth/token with client_id, client_secret, grant_type=client_credentials, and scope",
                    step2 = "Use returned access_token in Authorization: Bearer {access_token} header",
                    step3 = "GET /secure/oauth/protected (with access token)"
                },
                accessTokenExpiry = 3600,
                requiredParameters = new
                {
                    client_id = "client_123456",
                    client_secret = "client_secret_abcdefghij1234567890",
                    grant_type = "client_credentials",
                    scope = "space-separated list of scopes (e.g., 'read write delete')"
                }
            });
        }

        private class OAuthAccessToken
        {
            public string Token { get; set; }
            public string? RefreshToken { get; set; }
            public string Username { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        private class OAuthUser
        {
            public string Username { get; set; }
            public string UserId { get; set; }
            public string Email { get; set; }
            public string[] Permissions { get; set; }
            public string Role { get; set; }
        }
    }
}
