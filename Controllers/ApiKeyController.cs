using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Generate a new API key",
            Description = "Creates a new API key with `read` and `write` scopes, valid for one year. The full key value (`sk_test_...`) is only returned once at creation — store it securely. Use the key in the `X-API-Key` header for authenticated requests."
        )]
        [ProducesResponseType(201)]
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
        [SwaggerOperation(
            Summary = "List all API keys",
            Description = "Returns metadata for all active API keys. The actual key values are not returned — only IDs, scopes, and usage timestamps. Use `GET /api-keys/{keyId}` for details on a specific key."
        )]
        [ProducesResponseType(200)]
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
        [SwaggerOperation(
            Summary = "Get API key details",
            Description = "Returns metadata and permissions for a specific API key by its ID. Returns `404` if the key does not exist or has been revoked."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Get(
            [SwaggerParameter("The unique key ID, e.g. `key_123`. Returned when the key was created.", Required = true)] string keyId)
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
        [SwaggerOperation(
            Summary = "Revoke an API key",
            Description = "Permanently revokes an API key, preventing it from being used for authentication. This action is irreversible — use `POST /api-keys/{keyId}/rotate` if you need a replacement key."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Revoke(
            [SwaggerParameter("The unique key ID to revoke.", Required = true)] string keyId)
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
        [SwaggerOperation(
            Summary = "Rotate an API key",
            Description = "Generates a new API key to replace an existing one. The old key remains valid for 24 hours to allow a zero-downtime transition. After 24 hours, only the new key is accepted."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult Rotate(
            [SwaggerParameter("The unique key ID to rotate.", Required = true)] string keyId)
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
