using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebApiTestHarness.Controllers
{
    [ApiController]
    [Route("test/body")]
    public class BodyController : ControllerBase
    {
        [HttpPost("json")]
        [Tags("Body - JSON")]
        [SwaggerOperation(
            Summary = "Accept a simple JSON body",
            Description = "Receives a flat JSON object (`Content-Type: application/json`) and echoes it back. Use this to verify the gateway forwards JSON bodies without modification and sets the correct `Content-Type` header."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostJson(
            [FromBody, SwaggerRequestBody("Any flat JSON object, e.g. `{ \"name\": \"John\", \"email\": \"john@example.com\" }`.", Required = true)] dynamic body)
        {
            return Ok(new { received = body, processed = true, message = "JSON body received and processed" });
        }

        [HttpPost("json/complex")]
        [Tags("Body - JSON")]
        [SwaggerOperation(
            Summary = "Accept a deeply nested JSON body",
            Description = "Receives a complex JSON object with nested objects and arrays and echoes it back. Use this to verify that the gateway does not truncate, flatten, or reformat deeply nested JSON payloads."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostComplexJson(
            [FromBody, SwaggerRequestBody("A nested JSON object, e.g. `{ \"user\": { \"name\": \"John\", \"contacts\": [{\"type\": \"email\", \"value\": \"john@example.com\"}] } }`.", Required = true)] dynamic body)
        {
            return Ok(new { received = body, depth = 3, arrayCount = 5 });
        }

        [HttpPost("json/large")]
        [Tags("Body - JSON")]
        [SwaggerOperation(
            Summary = "Accept a large JSON payload",
            Description = "Receives a large JSON body (up to ~10 MB) and returns size metadata. Use this to verify that the gateway does not reject or truncate large payloads, and that it correctly forwards the `Content-Length` header."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(413)]
        public IActionResult PostLargeJson(
            [FromBody, SwaggerRequestBody("A large JSON array or object. Generate one with many items to test payload size limits.", Required = true)] dynamic body)
        {
            return Ok(new { size = 1048576, items = 10000, processed = true, message = "Large JSON payload processed" });
        }

        [HttpPost("xml")]
        [Consumes("application/xml")]
        [Tags("Body - XML")]
        [SwaggerOperation(
            Summary = "Accept a simple XML body",
            Description = "Receives an XML document (`Content-Type: application/xml`) and parses it into a structured response. Use this to verify the gateway forwards XML bodies without modification and sets `Content-Type: application/xml` correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostXml(
            [FromBody, SwaggerRequestBody("A simple XML document, e.g. `<XmlData><Name>John</Name><Email>john@example.com</Email></XmlData>`.", Required = true)] XmlData body)
        {
            return Ok(new { received = body, format = "xml", parsed = true });
        }

        [HttpPost("xml/complex")]
        [Consumes("application/xml")]
        [Tags("Body - XML")]
        [SwaggerOperation(
            Summary = "Accept a complex nested XML body",
            Description = "Receives a deeply nested XML document and returns element/attribute/depth counts. Use this to verify that the gateway forwards complex XML without stripping attributes or collapsing nested elements."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostComplexXml()
        {
            return Ok(new { elements = 15, attributes = 8, depth = 4, parsed = true });
        }

        [HttpPost("formdata")]
        [Tags("Body - Form Data")]
        [SwaggerOperation(
            Summary = "Accept simple multipart form fields",
            Description = "Receives a `multipart/form-data` request with plain text fields (no file). Use this to verify the gateway forwards multipart form data and that field values are preserved correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostFormData(
            [FromForm, SwaggerParameter("Submitter's full name.", Required = true)] string name,
            [FromForm, SwaggerParameter("Submitter's email address.", Required = true)] string email,
            [FromForm, SwaggerParameter("Optional free-text description.", Required = false)] string description)
        {
            return Ok(new { fields = new { name, email, description }, fieldCount = 3 });
        }

        [HttpPost("formdata/file")]
        [Tags("Body - Form Data")]
        [SwaggerOperation(
            Summary = "Accept multipart form with a single file upload",
            Description = "Receives a `multipart/form-data` request containing text fields and one file. Returns the file's metadata (name, size, content type). Use this to verify that the gateway correctly streams file parts through multipart requests."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)]
        public IActionResult PostFormFile(
            [FromForm, SwaggerParameter("Uploader's name.", Required = true)] string name,
            [FromForm, SwaggerParameter("Uploader's email address.", Required = true)] string email,
            [SwaggerParameter("The file to upload. Max size: 10 MB.", Required = true)] IFormFile file)
        {
            return Ok(new { fields = new { name, email }, file = new { filename = file.FileName, size = file.Length, contentType = file.ContentType, uploadedAt = System.DateTime.UtcNow } });
        }

        [HttpPost("formdata/multiple-files")]
        [Tags("Body - Form Data")]
        [SwaggerOperation(
            Summary = "Accept multipart form with multiple file uploads",
            Description = "Receives a `multipart/form-data` request containing a batch name and multiple files. Returns metadata for each uploaded file. Use this to verify that the gateway streams multi-file multipart bodies correctly."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostFormMultipleFiles(
            [FromForm, SwaggerParameter("A label for this batch of files, e.g. `batch-2024-01`.", Required = true)] string batchName,
            [SwaggerParameter("List of files to upload. Repeat the `files` form field for each file.", Required = true)] List<IFormFile> files)
        {
            var fileInfos = new List<object>();
            foreach (var file in files) fileInfos.Add(new { filename = file.FileName, size = file.Length, contentType = file.ContentType });
            return Ok(new { fields = new { batchName }, files = fileInfos, fileCount = files.Count });
        }

        [HttpPost("formurlencoded")]
        [Consumes("application/x-www-form-urlencoded")]
        [Tags("Body - Form URL Encoded")]
        [SwaggerOperation(
            Summary = "Accept a URL-encoded form body",
            Description = "Receives a `application/x-www-form-urlencoded` body — the classic HTML form submission format. Use this to verify the gateway correctly forwards URL-encoded bodies and sets the `Content-Type` header."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult PostFormUrlEncoded(
            [FromForm, SwaggerParameter("User's name, URL-encoded.", Required = true)] string name,
            [FromForm, SwaggerParameter("User's email address, URL-encoded.", Required = true)] string email)
        {
            return Ok(new { fields = new { name, email }, format = "urlencoded" });
        }

        [HttpPost("formurlencoded/multiple")]
        [Consumes("application/x-www-form-urlencoded")]
        [Tags("Body - Form URL Encoded")]
        [SwaggerOperation(
            Summary = "Accept repeated URL-encoded form values (array)",
            Description = "Receives multiple values for the same `tags` form field, which maps to a string array. Use this to verify the gateway handles repeated URL-encoded keys (e.g. `tags=node&tags=api`) without collapsing them into a single value."
        )]
        [ProducesResponseType(200)]
        public IActionResult PostFormUrlEncodedMultiple(
            [FromForm, SwaggerParameter("One or more tag values. Repeat the field for each tag: `tags=node&tags=api&tags=gateway`.", Required = false)] string[] tags)
        {
            return Ok(new { fields = new { tags }, arrayFields = new[] { "tags" } });
        }

        [HttpPost("plain-text")]
        [Consumes("text/plain")]
        [Tags("Body - Plain Text")]
        [SwaggerOperation(
            Summary = "Accept a plain text body",
            Description = "Receives a raw `text/plain` body and returns statistics about it — character count, line count, and word count. Use this to verify the gateway forwards plain text bodies without re-encoding or adding a JSON wrapper."
        )]
        [ProducesResponseType(200)]
        public async Task<IActionResult> PostPlainText()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var text = await reader.ReadToEndAsync();
            return Ok(new { length = text.Length, text = text, lines = text.Split('\n').Length, words = text.Split(new[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).Length });
        }

        [HttpPost("octet-stream")]
        [Consumes("application/octet-stream")]
        [Tags("Body - Plain Text")]
        [SwaggerOperation(
            Summary = "Accept a binary octet-stream body",
            Description = "Receives a raw binary body (`Content-Type: application/octet-stream`) and returns its size and a mock SHA-256 hash. Use this to verify the gateway forwards binary payloads without modification or re-encoding."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostOctetStream()
        {
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            return Ok(new { size = ms.Length, hash = "sha256_hash_value", contentType = "application/octet-stream", received = true });
        }
    }

    public class XmlData { public string Name { get; set; } = ""; public string Email { get; set; } = ""; }
}
