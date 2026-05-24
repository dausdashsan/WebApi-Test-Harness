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
        private static readonly Dictionary<string, OAuthAuthorizationCode> AuthorizationCodes = new();
        private static readonly Dictionary<string, OAuthAccessToken> AccessTokens = new();

        private static readonly Dictionary<string, OAuthUser> OAuthUsers = new()
        {
            { "admin", new OAuthUser { Username = "admin", UserId = "user_001", Email = "admin@example.com", Permissions = new[] { "read", "write", "delete" }, Role = "admin" } },
            { "user", new OAuthUser { Username = "user", UserId = "user_002", Email = "user@example.com", Permissions = new[] { "read", "write" }, Role = "user" } },
            { "guest", new OAuthUser { Username = "guest", UserId = "user_003", Email = "guest@example.com", Permissions = new[] { "read" }, Role = "guest" } }
        };

        private const string ClientId = "client_123456";
        private const string ClientSecret = "client_secret_abcdefghij1234567890";
        private const string RedirectUri = "http://localhost:3000/callback";

        [HttpPost("authorize")]
        [SwaggerOperation(
            Summary = "OAuth 2.0 Authorization endpoint",
            Description = "Initiates OAuth 2.0 authorization flow. Request an authorization code by providing client_id, redirect_uri, and response_type=code. Pass ?username=admin to authorize as admin (or user, guest)."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Authorize(
            [FromQuery, SwaggerParameter("OAuth 2.0 client identifier", Required = true)] string client_id,
            [FromQuery, SwaggerParameter("Redirect URI where authorization code will be sent", Required = true)] string redirect_uri,
            [FromQuery, SwaggerParameter("Must be 'code' for authorization code flow", Required = true)] string response_type,
            [FromQuery, SwaggerParameter("Username to authorize (admin, user, guest)", Required = false)] string username = "user",
            [FromQuery, SwaggerParameter("Optional state parameter for CSRF protection", Required = false)] string? state = null)
        {
            if (client_id != ClientId || redirect_uri != RedirectUri || response_type != "code")
            {
                return BadRequest(new { error = "invalid_request", error_description = "Invalid client_id, redirect_uri, or response_type" });
            }

            if (!OAuthUsers.ContainsKey(username))
            {
                return BadRequest(new { error = "invalid_request", error_description = "Invalid username" });
            }

            var authCode = "auth_code_" + Guid.NewGuid().ToString().Substring(0, 20);
            var codeExpiry = DateTime.UtcNow.AddMinutes(10);

            AuthorizationCodes[authCode] = new OAuthAuthorizationCode
            {
                Code = authCode,
                ClientId = client_id,
                Username = username,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = codeExpiry,
                State = state
            };

            var redirectUrl = $"{redirect_uri}?code={authCode}";
            if (!string.IsNullOrEmpty(state))
            {
                redirectUrl += $"&state={state}";
            }

            return Ok(new
            {
                authorizationCode = authCode,
                redirectUrl = redirectUrl,
                expiresIn = 600,
                message = "Authorization code generated. Redirect to the URL above with the authorization code."
            });
        }

        [HttpPost("token")]
        [SwaggerOperation(
            Summary = "OAuth 2.0 Token endpoint",
            Description = "Exchanges an authorization code for an access token. Requires client_id, client_secret, authorization code, and grant_type=authorization_code."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult Token(
            [FromForm, SwaggerParameter("OAuth 2.0 client identifier", Required = true)] string client_id,
            [FromForm, SwaggerParameter("OAuth 2.0 client secret", Required = true)] string client_secret,
            [FromForm, SwaggerParameter("Authorization code from /authorize endpoint", Required = true)] string code,
            [FromForm, SwaggerParameter("Must be 'authorization_code'", Required = true)] string grant_type,
            [FromForm, SwaggerParameter("Must match the redirect_uri used in /authorize", Required = true)] string redirect_uri)
        {
            if (client_id != ClientId || client_secret != ClientSecret || grant_type != "authorization_code")
            {
                return BadRequest(new { error = "invalid_client", error_description = "Invalid client_id, client_secret, or grant_type" });
            }

            if (!AuthorizationCodes.TryGetValue(code, out var authCode))
            {
                return BadRequest(new { error = "invalid_grant", error_description = "Authorization code not found or has expired" });
            }

            if (DateTime.UtcNow > authCode.ExpiresAt)
            {
                AuthorizationCodes.Remove(code);
                return BadRequest(new { error = "invalid_grant", error_description = "Authorization code has expired" });
            }

            var accessToken = "access_token_" + Guid.NewGuid().ToString().Substring(0, 32);
            var refreshToken = "refresh_token_" + Guid.NewGuid().ToString().Substring(0, 32);
            var expiresIn = 3600;

            AccessTokens[accessToken] = new OAuthAccessToken
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Username = authCode.Username,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn)
            };

            AuthorizationCodes.Remove(code);

            return Ok(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = expiresIn,
                refresh_token = refreshToken,
                scope = "read write delete"
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
            Description = "Returns information about OAuth 2.0 authentication. No authentication required. Client ID: client_123456, Client Secret: client_secret_abcdefghij1234567890"
        )]
        [ProducesResponseType(200)]
        public IActionResult GetUserInfo()
        {
            return Ok(new
            {
                authType = "OAuth 2.0",
                description = "OAuth 2.0 Authorization Code Flow",
                clientId = "client_123456",
                clientSecret = "client_secret_abcdefghij1234567890",
                redirectUri = "http://localhost:3000/callback",
                validUsers = new[]
                {
                    new { username = "admin", email = "admin@example.com", role = "admin", permissions = new[] { "read", "write", "delete" } },
                    new { username = "user", email = "user@example.com", role = "user", permissions = new[] { "read", "write" } },
                    new { username = "guest", email = "guest@example.com", role = "guest", permissions = new[] { "read" } }
                },
                flow = new
                {
                    step1 = "POST /secure/oauth/authorize?client_id=client_123456&redirect_uri=http://localhost:3000/callback&response_type=code&username=admin",
                    step2 = "POST /secure/oauth/token (with authorization code)",
                    step3 = "GET /secure/oauth/protected (with access token)"
                },
                authorizationCodeExpiry = 600,
                accessTokenExpiry = 3600
            });
        }

        private class OAuthAuthorizationCode
        {
            public string Code { get; set; }
            public string ClientId { get; set; }
            public string Username { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string State { get; set; }
        }

        private class OAuthAccessToken
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
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
