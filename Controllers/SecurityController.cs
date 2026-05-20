using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("security")]
    public class SecurityController : ControllerBase
    {
        [HttpGet("validate")]
        public IActionResult Validate()
        {
            return Ok(new
            {
                authenticated = true,
                user = "admin",
                userId = Guid.NewGuid().ToString(),
                permissions = new[] { "read", "write", "delete" },
                scopes = new[] { "*" },
                authType = "bearer"
            });
        }

        [HttpGet("permissions")]
        public IActionResult GetPermissions()
        {
            return Ok(new
            {
                user = "admin",
                userId = Guid.NewGuid().ToString(),
                permissions = new[] { "read", "write", "delete", "admin" },
                resourcePermissions = new
                {
                    users = new[] { "read", "write", "delete" },
                    files = new[] { "read", "write" },
                    settings = new[] { "read", "write", "admin" }
                }
            });
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(new
            {
                user = "admin",
                userId = Guid.NewGuid().ToString(),
                roles = new[] { "admin", "user" },
                roleDetails = new[]
                {
                    new
                    {
                        role = "admin",
                        permissions = new[] { "*" },
                        assignedAt = "2024-01-01T00:00:00Z"
                    }
                }
            });
        }
    }
}
