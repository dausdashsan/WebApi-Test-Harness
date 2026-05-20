using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test")]
    public class ParametersController : ControllerBase
    {
        // ── Path Parameters ─────────────────────────────────────────────────

        [HttpGet("path/{id:int}")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET with integer path parameter",
            Description = "Tests routing and extraction of a typed integer path parameter. Returns `400` if the value is not a valid integer, and `404` if it falls outside the known range. Use this to verify the gateway forwards integer path segments correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetByIntId(
            [SwaggerParameter("A positive integer resource ID, e.g. `123`.", Required = true)] int id)
        {
            return Ok(new
            {
                id = id,
                name = $"Item {id}",
                description = "Details for item",
                createdAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            });
        }

        [HttpGet("path/{uuid:guid}")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET with UUID path parameter",
            Description = "Tests routing and extraction of a UUID (GUID) path parameter. Returns `400` if the value is not a valid UUID format (`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`). Use this to verify the gateway correctly forwards UUID path segments."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetByUuid(
            [SwaggerParameter("A valid UUID v4, e.g. `550e8400-e29b-41d4-a716-446655440000`.", Required = true)] Guid uuid)
        {
            return Ok(new
            {
                uuid = uuid,
                data = new { info = "Resource data" }
            });
        }

        [HttpGet("path/{category}/{subcategory}/{itemId}")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET with multiple path parameters",
            Description = "Tests routing with three consecutive path segments. All three values are extracted and echoed back. Use this to verify that the gateway preserves multi-segment paths and correctly maps each segment to its named parameter."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetMultiPath(
            [SwaggerParameter("Top-level category, e.g. `electronics`.", Required = true)] string category,
            [SwaggerParameter("Sub-category within the parent, e.g. `phones`.", Required = true)] string subcategory,
            [SwaggerParameter("Specific item identifier within the subcategory, e.g. `iphone14`.", Required = true)] string itemId)
        {
            return Ok(new
            {
                category = category,
                subcategory = subcategory,
                itemId = itemId,
                product = new
                {
                    name = itemId,
                    price = 999.99
                }
            });
        }

        [HttpGet("path/{slug}")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET with URL slug path parameter",
            Description = "Tests routing with a URL-safe slug (lowercase letters, digits, hyphens). Use this to verify the gateway forwards slug-style path parameters without percent-encoding or modification."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetBySlug(
            [SwaggerParameter("A URL-safe slug string, e.g. `my-product` or `api-gateway-guide`.", Required = true)] string slug)
        {
            if (slug.Contains("details") || slug == "simple" || slug == "querystring" || slug == "search" || slug == "items" || slug == "body" || slug == "headers" || slug == "error" || slug == "rate-limited" || slug == "idempotent" || slug == "compression" || slug == "metadata" || slug == "tracing") return NotFound();
            return Ok(new
            {
                slug = slug,
                title = "My Product",
                description = "Product description"
            });
        }

        [HttpGet("path/{slug}/details")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET with slug path parameter and static suffix",
            Description = "Tests a route pattern where a dynamic path segment is followed by a static suffix (`/details`). Use this to verify the gateway correctly disambiguates between `/path/{slug}` and `/path/{slug}/details` route patterns."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult GetSlugDetails(
            [SwaggerParameter("A URL-safe slug identifying the resource, e.g. `my-product`.", Required = true)] string slug)
        {
            return Ok(new
            {
                slug = slug,
                details = new
                {
                    description = "Full description",
                    specifications = new { weight = "1kg" }
                }
            });
        }

        [HttpGet("path/{id}/combined")]
        [Tags("Path Parameters")]
        [SwaggerOperation(
            Summary = "GET combining path parameter and query parameters",
            Description = "Tests a route that uses both a path parameter and query parameters simultaneously. Use this to verify that the gateway forwards path segments without stripping query parameters, and that both are independently accessible on the backend."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetCombined(
            [SwaggerParameter("Resource ID in the path, e.g. `123`.", Required = true)] string id,
            [FromQuery, SwaggerParameter("Controls how much detail to include. Use `full` to include nested objects; omit for a summary view.", Required = false)] string expand = "none",
            [FromQuery, SwaggerParameter("Comma-separated list of field names to include in the response, e.g. `id,name,status`.", Required = false)] string fields = "")
        {
            return Ok(new
            {
                id = id,
                name = "Item Name",
                expand = expand,
                fields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries),
                data = new { }
            });
        }

        // ── Query Parameters ─────────────────────────────────────────────────

        [HttpGet("querystring")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Paginated list with filter query parameter",
            Description = "Tests basic query parameter handling — pagination via `page`/`limit` and a simple string filter. Returns a paginated result set. Use this to verify the gateway preserves all query parameters and that pagination metadata is returned correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetQueryString(
            [FromQuery, SwaggerParameter("Page number (1-based). Defaults to `1`.", Required = false)] int page = 1,
            [FromQuery, SwaggerParameter("Number of items per page (1–100). Defaults to `10`.", Required = false)] int limit = 10,
            [FromQuery, SwaggerParameter("Status filter string, e.g. `active`, `inactive`, or `pending`.", Required = false)] string filter = "")
        {
            return Ok(new
            {
                page = page,
                limit = limit,
                total = 150,
                totalPages = 15,
                data = new[] { new { id = 1, name = "Item 1", status = filter } }
            });
        }

        [HttpGet("querystring/multiple")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Multi-value query parameter (array)",
            Description = "Tests repeated query parameters that map to an array — e.g. `?tag=node&tag=api&tag=gateway`. Use this to verify the gateway forwards multi-value parameters as an array rather than collapsing them into a single string."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetMultipleQuery(
            [FromQuery, SwaggerParameter("One or more tags to filter by. Repeat the parameter for multiple values, e.g. `?tag=node&tag=api`.", Required = false)] string[] tag)
        {
            return Ok(new
            {
                tags = tag,
                results = new[] { new { id = 1, title = "Node.js Guide", tags = tag.Take(2) } },
                count = tag.Length
            });
        }

        [HttpGet("querystring/complex")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Nested / bracket-notation query parameters",
            Description = "Tests query parameters in bracket notation: `?filter[status]=active&filter[type]=api&sort=-created`. Use this to verify the gateway forwards bracket-notation parameters without URL-decoding the brackets into different keys."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult GetComplexQuery()
        {
            var filters = new Dictionary<string, string>();
            foreach (var key in Request.Query.Keys)
            {
                if (key.StartsWith("filter["))
                {
                    var field = key.Replace("filter[", "").Replace("]", "");
                    filters[field] = Request.Query[key]!;
                }
            }

            return Ok(new
            {
                filters = filters,
                sortBy = Request.Query["sort"].ToString(),
                results = new[] { new { id = 1, status = "active", type = "api", created = DateTime.UtcNow } }
            });
        }

        [HttpGet("querystring/optional")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Optional query parameters with defaults",
            Description = "Tests an endpoint where all query parameters are optional and have sensible defaults. Use this to verify the gateway does not inject or strip parameters when none are provided by the caller."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetOptionalQuery(
            [FromQuery, SwaggerParameter("Free-text search query. Omit to return all results.", Required = false)] string q = "",
            [FromQuery, SwaggerParameter("Maximum number of results to return. Defaults to `20`.", Required = false)] int limit = 20)
        {
            return Ok(new
            {
                query = q,
                limit = limit,
                offset = 0,
                results = new[] { new { id = 1, name = "Search Result 1" } },
                total = 45
            });
        }

        [HttpGet("search")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Full-text search with sorting and pagination",
            Description = "Tests a richer query parameter set: keyword search, result limit, offset-based pagination, and sort order. The `sort` field follows the convention where a leading `-` means descending, e.g. `-date` sorts newest first."
        )]
        [ProducesResponseType(200)]
        public IActionResult Search(
            [FromQuery, SwaggerParameter("Keyword to search for across all result fields.", Required = false)] string q = "",
            [FromQuery, SwaggerParameter("Maximum number of results to return (1–500). Defaults to `50`.", Required = false)] int limit = 50,
            [FromQuery, SwaggerParameter("Number of results to skip before returning the page. Use with `limit` for pagination.", Required = false)] int offset = 0,
            [FromQuery, SwaggerParameter("Sort order. Prefix with `-` for descending, e.g. `-date` (newest first) or `name` (A–Z).", Required = false)] string sort = "-date")
        {
            return Ok(new
            {
                query = q,
                limit = limit,
                offset = offset,
                sort = sort,
                results = new[] { new { id = 1, title = "Result 1", date = DateTime.UtcNow } },
                total = 123
            });
        }

        [HttpGet("items")]
        [Tags("Query Parameters")]
        [SwaggerOperation(
            Summary = "Field selection and expansion",
            Description = "Tests sparse fieldsets (`fields=id,name`) and relationship expansion (`expand=full`). Use this to verify the gateway forwards these projection parameters without modification so the backend can shape the response accordingly."
        )]
        [ProducesResponseType(200)]
        public IActionResult GetItems(
            [FromQuery, SwaggerParameter("Comma-separated list of fields to include in each result object, e.g. `id,name,description`.", Required = false)] string fields = "",
            [FromQuery, SwaggerParameter("Pass `full` to include nested related objects (details, metadata) in the response.", Required = false)] string expand = "")
        {
            return Ok(new
            {
                items = new[]
                {
                    new
                    {
                        id = 1,
                        name = "Item 1",
                        description = "Description text",
                        expanded = expand == "full" ? new { details = new { }, metadata = new { } } : null
                    }
                }
            });
        }
    }
}
