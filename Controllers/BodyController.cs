using Microsoft.AspNetCore.Mvc;
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
        public IActionResult PostJson([FromBody] dynamic body)
        {
            return Ok(new { received = body, processed = true, message = "JSON body received and processed" });
        }

        [HttpPost("json/complex")]
        [Tags("Body - JSON")]
        public IActionResult PostComplexJson([FromBody] dynamic body)
        {
            return Ok(new { received = body, depth = 3, arrayCount = 5 });
        }

        [HttpPost("json/large")]
        [Tags("Body - JSON")]
        public IActionResult PostLargeJson([FromBody] dynamic body)
        {
            return Ok(new { size = 1048576, items = 10000, processed = true, message = "Large JSON payload processed" });
        }

        [HttpPost("xml")]
        [Consumes("application/xml")]
        [Tags("Body - XML")]
        public IActionResult PostXml([FromBody] XmlData body)
        {
            return Ok(new { received = body, format = "xml", parsed = true });
        }

        [HttpPost("xml/complex")]
        [Consumes("application/xml")]
        [Tags("Body - XML")]
        public IActionResult PostComplexXml()
        {
            return Ok(new { elements = 15, attributes = 8, depth = 4, parsed = true });
        }

        [HttpPost("formdata")]
        [Tags("Body - Form Data")]
        public IActionResult PostFormData([FromForm] string name, [FromForm] string email, [FromForm] string description)
        {
            return Ok(new { fields = new { name, email, description }, fieldCount = 3 });
        }

        [HttpPost("formdata/file")]
        [Tags("Body - Form Data")]
        public IActionResult PostFormFile([FromForm] string name, [FromForm] string email, IFormFile file)
        {
            return Ok(new { fields = new { name, email }, file = new { filename = file.FileName, size = file.Length, contentType = file.ContentType, uploadedAt = System.DateTime.UtcNow } });
        }

        [HttpPost("formdata/multiple-files")]
        [Tags("Body - Form Data")]
        public IActionResult PostFormMultipleFiles([FromForm] string batchName, List<IFormFile> files)
        {
            var fileInfos = new List<object>();
            foreach (var file in files) fileInfos.Add(new { filename = file.FileName, size = file.Length, contentType = file.ContentType });
            return Ok(new { fields = new { batchName }, files = fileInfos, fileCount = files.Count });
        }

        [HttpPost("formurlencoded")]
        [Consumes("application/x-www-form-urlencoded")]
        [Tags("Body - Form URL Encoded")]
        public IActionResult PostFormUrlEncoded([FromForm] string name, [FromForm] string email)
        {
            return Ok(new { fields = new { name, email }, format = "urlencoded" });
        }

        [HttpPost("formurlencoded/multiple")]
        [Consumes("application/x-www-form-urlencoded")]
        [Tags("Body - Form URL Encoded")]
        public IActionResult PostFormUrlEncodedMultiple([FromForm] string[] tags)
        {
            return Ok(new { fields = new { tags }, arrayFields = new[] { "tags" } });
        }

        [HttpPost("plain-text")]
        [Consumes("text/plain")]
        [Tags("Body - Plain Text")]
        public async Task<IActionResult> PostPlainText()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var text = await reader.ReadToEndAsync();
            return Ok(new { length = text.Length, text = text, lines = text.Split('\n').Length, words = text.Split(new[] { ' ', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).Length });
        }

        [HttpPost("octet-stream")]
        [Consumes("application/octet-stream")]
        [Tags("Body - Plain Text")]
        public async Task<IActionResult> PostOctetStream()
        {
            using var ms = new MemoryStream();
            await Request.Body.CopyToAsync(ms);
            return Ok(new { size = ms.Length, hash = "sha256_hash_value", contentType = "application/octet-stream", received = true });
        }
    }

    public class XmlData { public string Name { get; set; } = ""; public string Email { get; set; } = ""; }
}
