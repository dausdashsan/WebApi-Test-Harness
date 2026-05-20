using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        [SwaggerOperation(
            Summary = "Upload a single file",
            Description = "Accepts a `multipart/form-data` request with a single file and optional metadata fields. Returns the file's assigned ID and a download URL. Max file size: 10 MB. Use this to verify the gateway streams file uploads to the backend."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)]
        public IActionResult UploadSingle(
            [SwaggerParameter("The file to upload.", Required = true)] IFormFile file,
            [FromForm, SwaggerParameter("Human-readable description of the file's contents.", Required = false)] string description,
            [FromForm, SwaggerParameter("Comma-separated tags to associate with the file, e.g. `invoice,2024`.", Required = false)] string tags)
        {
            return Created("", new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, fileSize = file.Length, contentType = file.ContentType, uploadedAt = DateTime.UtcNow, url = $"/api/files/{Guid.NewGuid()}/download" });
        }

        [HttpPost("upload/batch")]
        [Tags("File Upload")]
        [SwaggerOperation(
            Summary = "Upload multiple files in a single request",
            Description = "Accepts a `multipart/form-data` request with multiple files under the `files` field and a batch label. Returns per-file upload results. Use this to verify the gateway handles multi-file multipart uploads."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult UploadBatch(
            [SwaggerParameter("List of files to upload. Repeat the `files` field for each file.", Required = true)] List<IFormFile> files,
            [FromForm, SwaggerParameter("A label for this batch, e.g. `invoices-jan-2024`.", Required = false)] string batchName)
        {
            var fileResults = new List<object>();
            foreach (var file in files) fileResults.Add(new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, fileSize = file.Length, status = "success" });
            return Created("", new { batchId = "batch-" + Guid.NewGuid().ToString(), batchName, filesCount = files.Count, successCount = files.Count, failureCount = 0, uploadedAt = DateTime.UtcNow, files = fileResults });
        }

        [HttpPost("upload/chunked")]
        [Tags("File Upload")]
        [SwaggerOperation(
            Summary = "Upload a file chunk (resumable upload)",
            Description = "Supports resumable chunked uploads. Send each chunk sequentially with `chunkNumber` starting at 1. When `chunkNumber == totalChunks` the server assembles the file and returns `201 Created`. Otherwise returns `202 Accepted` with the URL for the next chunk."
        )]
        [ProducesResponseType(202)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult UploadChunked(
            [FromQuery, SwaggerParameter("Unique upload session ID. Generate a UUID for the first chunk and reuse it for all subsequent chunks.", Required = true)] string uploadId,
            [FromQuery, SwaggerParameter("1-based index of the current chunk being uploaded.", Required = true)] int chunkNumber,
            [FromQuery, SwaggerParameter("Total number of chunks the file is split into.", Required = true)] int totalChunks)
        {
            if (chunkNumber < totalChunks) return Accepted(new { uploadId, chunkNumber, totalChunks, status = "uploading", nextChunkUrl = $"/files/upload/chunked?uploadId={uploadId}&chunkNumber={chunkNumber + 1}" });
            return Created("", new { uploadId, fileId = Guid.NewGuid().ToString(), fileName = "largefile.zip", fileSize = 104857600, status = "complete", url = $"/api/files/{Guid.NewGuid()}/download" });
        }

        [HttpPost("upload/with-validation")]
        [Tags("File Upload")]
        [SwaggerOperation(
            Summary = "Upload a file with size and type validation",
            Description = "Uploads a file and validates it against caller-specified constraints before accepting it. Returns `400` if the file exceeds `maxSize` or is not one of the `allowedTypes`. Returns validation result details on success."
        )]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)]
        public IActionResult UploadWithValidation(
            [SwaggerParameter("The file to validate and upload.", Required = true)] IFormFile file,
            [FromQuery, SwaggerParameter("Maximum allowed file size in bytes, e.g. `10485760` for 10 MB.", Required = false)] long maxSize = 10485760,
            [FromQuery, SwaggerParameter("Comma-separated list of allowed MIME types, e.g. `image/jpeg,image/png,application/pdf`.", Required = false)] string allowedTypes = "image/jpeg,image/png,application/pdf")
        {
            return Created("", new { fileId = Guid.NewGuid().ToString(), fileName = file.FileName, validated = true, validation = new { size = "pass", type = "pass", virusScan = "pass" } });
        }

        [HttpGet("{fileId}/download")]
        [Tags("File Download")]
        [SwaggerOperation(
            Summary = "Download a file by ID",
            Description = "Returns the file content with appropriate `Content-Disposition` and `Content-Type` headers. The `disposition` parameter controls whether the browser downloads or displays the file. Supports `ETag` and `Last-Modified` headers for caching."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DownloadSingle(
            [SwaggerParameter("The unique file ID returned when the file was uploaded.", Required = true)] string fileId,
            [FromQuery, SwaggerParameter("When `true`, sets `Content-Disposition: inline` so browsers display the file rather than downloading it.", Required = false)] bool inline = false,
            [FromQuery, SwaggerParameter("Overrides the `Content-Disposition` header. Allowed values: `attachment` (default), `inline`, `preview`.", Required = false)] string disposition = "attachment")
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Dummy file content"));
            return File(stream, "application/pdf", "document.pdf", true);
        }

        [HttpGet("{fileId}/download-stream")]
        [Tags("File Download")]
        [SwaggerOperation(
            Summary = "Stream a file with HTTP range request support",
            Description = "Returns the file as a byte stream with `Accept-Ranges: bytes`. Supports HTTP `206 Partial Content` for range requests (`Range: bytes=0-1048575`). Use this to verify the gateway correctly proxies chunked streaming responses."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(206)]
        [ProducesResponseType(404)]
        public IActionResult DownloadStream(
            [SwaggerParameter("The unique file ID to stream.", Required = true)] string fileId)
        {
            var data = new byte[1024];
            new Random().NextBytes(data);
            return File(data, "application/octet-stream", true);
        }

        [HttpGet("{fileId}/download-conditional")]
        [Tags("File Download")]
        [SwaggerOperation(
            Summary = "Conditional download using ETag / If-Modified-Since",
            Description = "Supports conditional GET via `If-None-Match` (ETag) and `If-Modified-Since` headers. Returns `304 Not Modified` if the file has not changed, saving bandwidth. Use this to verify the gateway forwards conditional headers and correctly proxies `304` responses."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(304)]
        [ProducesResponseType(404)]
        public IActionResult DownloadConditional(
            [SwaggerParameter("The unique file ID to conditionally download.", Required = true)] string fileId)
        {
            var etag = "\"abc123def456\"";
            if (Request.Headers.IfNoneMatch == etag) return StatusCode(304);
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Conditional content"));
            Response.Headers.ETag = etag;
            return File(stream, "application/pdf", "document.pdf");
        }

        [HttpPost("download/batch")]
        [Tags("File Download")]
        [SwaggerOperation(
            Summary = "Download multiple files as a ZIP archive",
            Description = "Accepts a list of file IDs and returns them bundled into a single ZIP archive. The archive filename is controlled by `archiveName`. Returns `400` if any of the provided IDs are invalid or not found."
        )]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DownloadBatch(
            [FromBody, SwaggerRequestBody("List of file IDs to include in the archive, plus an optional archive name.", Required = true)] BatchDownloadRequest request)
        {
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry("file1.txt");
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                await writer.WriteAsync("Batch file content");
            }
            return File(ms.ToArray(), "application/zip", $"{request.ArchiveName ?? "archive"}.zip");
        }
    }
}
