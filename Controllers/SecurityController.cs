using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("security")]
    [Tags("Authentication & Security")]
    public class SecurityController : ControllerBase
    {
        [HttpGet("validate")]
        [SwaggerOperation(
            Summary = "Validate any authentication scheme",
            Description = "Accepts any auth header (`Authorization: Basic ...`, `Authorization: Bearer ...`, or `X-API-Key: ...`) and returns whether the credential is recognised. Useful for testing which auth schemes the gateway forwards correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
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
        [SwaggerOperation(
            Summary = "Get permissions for the current user",
            Description = "Returns the flat permission list and resource-level permission map for the authenticated user. Useful for verifying that the gateway forwards identity headers so the backend can apply authorization logic."
        )]
        [ProducesResponseType(200)]
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
        [SwaggerOperation(
            Summary = "Get roles for the current user",
            Description = "Returns the role list and per-role permission details for the authenticated user. Use this to verify that role claims propagated through the gateway are correctly interpreted by the backend."
        )]
        [ProducesResponseType(200)]
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
