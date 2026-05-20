using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("api-keys")]
    [Tags("Authentication & Security")]
    public class ApiKeyController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult Generate()
        {
            return Created("", new
            {
                apiKey = "sk_test_4eC39HqLyjWDarhtT1ZdV7DO",
                keyId = "key_123",
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                expiresAt = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                scopes = new[] { "read", "write" }
            });
        }

        [HttpGet]
        public IActionResult List()
        {
            return Ok(new
            {
                total = 3,
                keys = new[]
                {
                    new
                    {
                        keyId = "key_123",
                        createdAt = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        expiresAt = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        lastUsed = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        scopes = new[] { "read", "write" }
                    }
                }
            });
        }

        [HttpGet("{keyId}")]
        public IActionResult Get(string keyId)
        {
            return Ok(new
            {
                keyId = keyId,
                createdAt = DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                expiresAt = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                lastUsed = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                scopes = new[] { "read", "write" },
                permissions = new[] { "GET", "POST" }
            });
        }

        [HttpDelete("{keyId}")]
        public IActionResult Revoke(string keyId)
        {
            return Ok(new
            {
                success = true,
                keyId = keyId,
                revokedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                message = "API key revoked"
            });
        }

        [HttpPost("{keyId}/rotate")]
        public IActionResult Rotate(string keyId)
        {
            return Ok(new
            {
                oldKeyId = keyId,
                newApiKey = "sk_test_newkey789",
                newKeyId = Guid.NewGuid().ToString(),
                expiresAt = DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                message = "API key rotated, old key remains valid for 24 hours"
            });
        }
    }
}
