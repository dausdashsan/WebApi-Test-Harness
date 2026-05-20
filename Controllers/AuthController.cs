using Microsoft.AspNetCore.Mvc;
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
        public IActionResult RegisterBasic([FromBody] dynamic body)
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
        public IActionResult Login([FromBody] dynamic body)
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
        public IActionResult Refresh([FromBody] dynamic body)
        {
            return Ok(new
            {
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.new_dummy_payload",
                expiresIn = 3600,
                refreshToken = "new_refresh_token_abc"
            });
        }

        [HttpGet("validate")]
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
