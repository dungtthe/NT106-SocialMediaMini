using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using SocialMediaMini.Common.Helpers;

namespace SocialMediaMini.API.Areas.Shared
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpGet("image/{fileName}")]
        public IActionResult ViewImage([FromRoute] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("Thiếu fileName");

            var filePath = Path.Combine(Utils.GetPathUploadImage(), fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Không tìm thấy ảnh");

            var mimeType = GetMimeType(filePath);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, mimeType);
        }

        private string GetMimeType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Không có file nào được gửi");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            string uploadPath = Utils.GetPathUploadImage();

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(ext))
                    {
                        return BadRequest($"Định dạng file '{file.FileName}' không được hỗ trợ. Chỉ cho phép .jpg, .jpeg, .png");
                    }

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedFileNames.Add(fileName);
                }
            }

            return Ok(savedFileNames);
        }
    }
}
