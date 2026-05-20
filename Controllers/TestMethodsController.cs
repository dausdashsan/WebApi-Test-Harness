using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Simple GET request",
            Description = "Baseline GET endpoint with no parameters. Use this to verify that the gateway forwards GET requests to the backend and returns the response unmodified."
        )]
        [ProducesResponseType(200)]
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
        [SwaggerOperation(
            Summary = "Simple POST request",
            Description = "Baseline POST endpoint that accepts a JSON body and returns `201 Created`. Use this to verify that the gateway forwards POST requests with a body and that the `Location` header is preserved."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult PostSimple(
            [FromBody, SwaggerRequestBody("Any JSON object. The `message` field is echoed back in the response.", Required = true)] dynamic body)
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
        [SwaggerOperation(
            Summary = "Full resource replacement (PUT)",
            Description = "Replaces an entire resource by ID. Optionally accepts a `force=true` query parameter to override validation. Use this to verify that the gateway forwards PUT requests, path parameters, query parameters, and request bodies correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult PutSimple(
            [SwaggerParameter("ID of the resource to replace.", Required = true)] string id,
            [FromBody, SwaggerRequestBody("Full replacement object. All fields are required for a PUT.", Required = true)] dynamic body,
            [FromQuery, SwaggerParameter("When `true`, skips validation and forces the update even if the resource is locked.", Required = false)] bool force = false)
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
        [SwaggerOperation(
            Summary = "Partial resource update (PATCH)",
            Description = "Applies a partial update to a resource — only the fields included in the body are modified. Returns the list of patched field names. Use this to verify that the gateway forwards PATCH requests and preserves partial JSON bodies."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult PatchSimple(
            [SwaggerParameter("ID of the resource to partially update.", Required = true)] string id,
            [FromBody, SwaggerRequestBody("A partial JSON object containing only the fields to update.", Required = true)] Dictionary<string, object> body)
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
        [SwaggerOperation(
            Summary = "Single field update via path (PATCH)",
            Description = "Updates a single named field on a resource by encoding the field name in the URL path. The new value is passed as a raw body. Use this to test gateway handling of PATCH with multi-segment paths."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult PatchField(
            [SwaggerParameter("ID of the resource to update.", Required = true)] string id,
            [SwaggerParameter("Name of the field to update, e.g. `name` or `status`.", Required = true)] string field,
            [FromBody, SwaggerRequestBody("The new value for the field as a raw JSON value (string, number, boolean, etc.).", Required = true)] object value)
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
        [SwaggerOperation(
            Summary = "Delete a resource",
            Description = "Deletes a resource by ID. Supports soft delete (default) or hard delete via the `hard` query parameter. Use this to verify that the gateway forwards DELETE requests, path parameters, and query parameters."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteSimple(
            [SwaggerParameter("ID of the resource to delete.", Required = true)] string id,
            [FromQuery, SwaggerParameter("When `true`, permanently removes the record (hard delete). When `false` or omitted, marks it as deleted but retains the data (soft delete).", Required = false)] bool hard = false)
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
