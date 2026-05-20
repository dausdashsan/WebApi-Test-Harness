using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestMethodsController : ControllerBase
    {
        [HttpGet("simple")]
        [Tags("HTTP Methods - GET")]
        public IActionResult GetSimple()
        {
            return Ok(new
            {
                method = "GET",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                message = "GET request successful"
            });
        }

        [HttpPost("simple")]
        [Tags("HTTP Methods - POST")]
        public IActionResult PostSimple([FromBody] dynamic body)
        {
            return Created("", new
            {
                id = Guid.NewGuid().ToString(),
                message = "hello world",
                status = "created",
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpPut("simple/{id}")]
        [Tags("HTTP Methods - PUT")]
        public IActionResult PutSimple(string id, [FromBody] dynamic body, [FromQuery] bool force = false)
        {
            return Ok(new
            {
                id = id,
                message = "updated message",
                status = "updated",
                updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                forced = force
            });
        }

        [HttpPatch("simple/{id}")]
        [Tags("HTTP Methods - PATCH")]
        public IActionResult PatchSimple(string id, [FromBody] Dictionary<string, object> body)
        {
            return Ok(new
            {
                id = id,
                message = "updated",
                partialUpdate = true,
                patchedFields = body.Keys,
                updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpPatch("simple/{id}/{field}")]
        [Tags("HTTP Methods - PATCH")]
        public IActionResult PatchField(string id, string field, [FromBody] object value)
        {
            return Ok(new
            {
                id = id,
                field = field,
                value = value,
                updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpDelete("simple/{id}")]
        [Tags("HTTP Methods - DELETE")]
        public IActionResult DeleteSimple(string id, [FromQuery] bool hard = false)
        {
            return Ok(new
            {
                success = true,
                id = id,
                deleteType = hard ? "hard" : "soft",
                deletedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                message = "Resource deleted"
            });
        }
    }
}
