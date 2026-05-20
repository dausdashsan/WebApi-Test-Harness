using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("auth")]
    [Tags("Authentication & Security")]
    public class AuthController : ControllerBase
    {
        [HttpPost("basic/register")]
        [SwaggerOperation(
            Summary = "Register a Basic Auth user",
            Description = "Creates a new user credential pair for Basic Authentication testing. The username/password can then be used with `POST /auth/basic/validate` or passed as `Authorization: Basic base64(username:password)` through the gateway."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(409)]
        public IActionResult RegisterBasic(
            [FromBody, SwaggerRequestBody("Credentials to register. `username` must be unique; `password` must be at least 8 characters.", Required = true)] dynamic body)
        {
            return Created("", new
            {
                userId = Guid.NewGuid().ToString(),
                username = "john",
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                permissions = new[] { "read", "write" }
            });
        }

        [HttpPost("basic/validate")]
        [SwaggerOperation(
            Summary = "Validate Basic Auth credentials",
            Description = "Validates a Basic Auth `Authorization` header. Send the header as `Authorization: Basic base64(username:password)`. Returns the resolved user identity and permissions if credentials are valid."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidateBasic()
        {
            return Ok(new
            {
                valid = true,
                userId = Guid.NewGuid().ToString(),
                username = "john",
                permissions = new[] { "read", "write" }
            });
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login and obtain a JWT Bearer token",
            Description = "Authenticates with username and password and returns a signed JWT access token plus a refresh token. Use the returned `token` as `Authorization: Bearer {token}` on subsequent requests. Default test credentials: `admin` / `admin`."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Login(
            [FromBody, SwaggerRequestBody("Login credentials. Use `admin`/`admin` for a pre-seeded admin account.", Required = true)] dynamic body)
        {
            return Ok(new
            {
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.dummy_payload",
                expiresIn = 3600,
                refreshToken = "refresh_token_xyz",
                tokenType = "Bearer"
            });
        }

        [HttpPost("refresh")]
        [SwaggerOperation(
            Summary = "Refresh an expired JWT token",
            Description = "Exchanges a valid refresh token for a new access token and a new refresh token. The old refresh token is invalidated after use. Use this to maintain a session without re-entering credentials."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Refresh(
            [FromBody, SwaggerRequestBody("The refresh token previously issued by `POST /auth/login` or a prior refresh call.", Required = true)] dynamic body)
        {
            return Ok(new
            {
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.new_dummy_payload",
                expiresIn = 3600,
                refreshToken = "new_refresh_token_abc"
            });
        }

        [HttpGet("validate")]
        [SwaggerOperation(
            Summary = "Validate a JWT Bearer token",
            Description = "Checks whether the JWT in the `Authorization: Bearer {token}` header is valid and not expired. Returns the decoded user identity and time remaining until expiry. Useful for testing gateway JWT validation."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult ValidateToken()
        {
            return Ok(new
            {
                valid = true,
                user = new
                {
                    userId = Guid.NewGuid().ToString(),
                    username = "admin",
                    roles = new[] { "admin" }
                },
                expiresIn = 3600,
                expiresAt = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("introspect")]
        [SwaggerOperation(
            Summary = "Introspect JWT claims",
            Description = "Returns all claims encoded in the JWT Bearer token — subject, roles, scopes, issued-at, and expiry timestamps. Useful for verifying what the gateway forwards after stripping or transforming tokens."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public IActionResult Introspect()
        {
            return Ok(new
            {
                sub = "user_123",
                username = "admin",
                email = "admin@example.com",
                roles = new[] { "admin" },
                scopes = new[] { "read", "write", "delete" },
                iat = 1705315200,
                exp = 1705318800
            });
        }

        [HttpPost("logout")]
        [SwaggerOperation(
            Summary = "Invalidate a JWT token (logout)",
            Description = "Adds the provided JWT to a server-side blocklist so it can no longer be used, even if it has not yet expired. Send the token as `Authorization: Bearer {token}`."
        )]
        [ProducesResponseType(200)]
        public IActionResult Logout()
        {
            return Ok(new
            {
                success = true,
                message = "Token invalidated",
                loggedOutAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }
    }
}
