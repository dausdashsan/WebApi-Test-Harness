using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test")]
    public class ParametersController : ControllerBase
    {
        [HttpGet("path/{id:int}")]
        [Tags("Path Parameters")]
        public IActionResult GetByIntId(int id)
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
        public IActionResult GetByUuid(Guid uuid)
        {
            return Ok(new
            {
                uuid = uuid,
                data = new { info = "Resource data" }
            });
        }

        [HttpGet("path/{category}/{subcategory}/{itemId}")]
        [Tags("Path Parameters")]
        public IActionResult GetMultiPath(string category, string subcategory, string itemId)
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
        public IActionResult GetBySlug(string slug)
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
        public IActionResult GetSlugDetails(string slug)
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
        public IActionResult GetCombined(string id, [FromQuery] string expand = "none", [FromQuery] string fields = "")
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

        [HttpGet("querystring")]
        [Tags("Query Parameters")]
        public IActionResult GetQueryString([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string filter = "")
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
        public IActionResult GetMultipleQuery([FromQuery] string[] tag)
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
        public IActionResult GetOptionalQuery([FromQuery] string q = "", [FromQuery] int limit = 20)
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
        public IActionResult Search([FromQuery] string q = "", [FromQuery] int limit = 50, [FromQuery] int offset = 0, [FromQuery] string sort = "-date")
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
        public IActionResult GetItems([FromQuery] string fields = "", [FromQuery] string expand = "")
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
