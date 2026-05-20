using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace WebApiTestHarness.Controllers
{
    public class BatchDownloadRequest
    {
        public List<string> FileIds { get; set; } = new List<string>();
        public string? ArchiveName { get; set; }
    }

    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        [HttpPost("upload")]
        [Tags("File Upload")]
        public IActionResult UploadSingle(IFormFile file, [FromForm] string description, [FromForm] string tags)
        {
            return Created("", new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, fileSize = file.Length, contentType = file.ContentType, uploadedAt = DateTime.UtcNow, url = $"/api/files/{Guid.NewGuid()}/download" });
        }

        [HttpPost("upload/batch")]
        [Tags("File Upload")]
        public IActionResult UploadBatch(List<IFormFile> files, [FromForm] string batchName)
        {
            var fileResults = new List<object>();
            foreach (var file in files) fileResults.Add(new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, fileSize = file.Length, status = "success" });
            return Created("", new { batchId = "batch-" + Guid.NewGuid().ToString(), batchName, filesCount = files.Count, successCount = files.Count, failureCount = 0, uploadedAt = DateTime.UtcNow, files = fileResults });
        }

        [HttpPost("upload/chunked")]
        [Tags("File Upload")]
        public IActionResult UploadChunked([FromQuery] string uploadId, [FromQuery] int chunkNumber, [FromQuery] int totalChunks)
        {
            if (chunkNumber < totalChunks) return Accepted(new { uploadId, chunkNumber, totalChunks, status = "uploading", nextChunkUrl = $"/files/upload/chunked?uploadId={uploadId}&chunkNumber={chunkNumber + 1}" });
            return Created("", new { uploadId, fileId = Guid.NewGuid().ToString(), fileName = "largefile.zip", fileSize = 104857600, status = "complete", url = $"/api/files/{Guid.NewGuid()}/download" });
        }

        [HttpPost("upload/with-validation")]
        [Tags("File Upload")]
        public IActionResult UploadWithValidation(IFormFile file, [FromQuery] long maxSize, [FromQuery] string allowedTypes)
        {
            return Created("", new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, validated = true, validation = new { size = "pass", type = "pass", virusScan = "pass" } });
        }

        [HttpGet("{fileId}/download")]
        [Tags("File Download")]
        public IActionResult DownloadSingle(string fileId, [FromQuery] bool inline = false, [FromQuery] string disposition = "attachment")
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Dummy file content"));
            return File(stream, "application/pdf", "document.pdf", true);
        }

        [HttpGet("{fileId}/download-stream")]
        [Tags("File Download")]
        public IActionResult DownloadStream(string fileId)
        {
            var data = new byte[1024];
            new Random().NextBytes(data);
            return File(data, "application/octet-stream", true);
        }

        [HttpGet("{fileId}/download-conditional")]
        [Tags("File Download")]
        public IActionResult DownloadConditional(string fileId)
        {
            var etag = "\"abc123def456\"";
            if (Request.Headers.IfNoneMatch == etag) return StatusCode(304);
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Conditional content"));
            Response.Headers.ETag = etag;
            return File(stream, "application/pdf", "document.pdf");
        }

        [HttpPost("download/batch")]
        [Tags("File Download")]
        public async Task<IActionResult> DownloadBatch([FromBody] BatchDownloadRequest request)
        {
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry("file1.txt");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                await writer.WriteAsync("Batch file content");
            }
            return File(ms.ToArray(), "application/zip", $"archive.zip");
        }
    }
}
