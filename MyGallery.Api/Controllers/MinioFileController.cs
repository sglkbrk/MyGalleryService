using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyGallery.Services.Interfaces;


namespace MyGallery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MinioFileController : ControllerBase
    {
        private readonly IMinioFileService _minioFileService;

        public MinioFileController(IMinioFileService minioFileService)
        {
            _minioFileService = minioFileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            var key = await _minioFileService.UploadFileAsync(file);
            return Ok(new { Key = key, Url = _minioFileService.GetFileUrl(key) });
        }

        [HttpGet("download/{key}")]
        public async Task<IActionResult> Download(string key)
        {
            var stream = await _minioFileService.GetFileAsync(key);
            // İçerik tipini belirlemek için dosya uzantısına göre ayarlayabilirsiniz
            Response.Headers["Cache-Control"] = "public,max-age=2592000"; // 1 gün
            Response.Headers["Expires"] = DateTime.UtcNow.AddMonths(1).ToString("R"); // 1 gün sonra
            Response.Headers["ETag"] = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return File(stream, "application/octet-stream", key);
        }

        [HttpGet("url/{key}")]
        public IActionResult GetUrl(string key)
        {
            var url = _minioFileService.GetFileUrl(key);
            return Ok(new { Url = url });
        }
    }
}
