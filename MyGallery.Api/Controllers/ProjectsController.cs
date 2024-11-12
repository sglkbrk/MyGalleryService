using Microsoft.AspNetCore.Mvc;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using Microsoft.AspNetCore.Authorization;

namespace MyGallery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;
        private readonly IMinioFileService _minioFileService;

        public ProjectsController(IProjectsService projectsService, IMinioFileService minioFileService)
        {
            _projectsService = projectsService;
            _minioFileService = minioFileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Projects>>> GetProjectsItems()
        {
            return Ok(await _projectsService.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Projects>> PostProjectsItem(Projects item)
        {
            await _projectsService.AddAsync(item);
            return CreatedAtAction(nameof(GetProjectsItems), new { id = item.Id }, item);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Projects>> GetProjectsItem(string slug)
        {
            var item = await _projectsService.GetProjectWithPhotosAsync(slug);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        [HttpGet("{slug}/{format}")]
        public async Task<ActionResult<Projects>> GetProjectsItem(string slug, Format format)
        {
            var item = await _projectsService.GetProjectAllPhotosAsync(slug, format);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        [HttpGet("recent/{count}")]
        public async Task<ActionResult<Projects>> GetProjectsItem(int count)
        {
            var item = await _projectsService.GetRecentProject(count);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpGet("home/{count}")]
        public async Task<ActionResult<Projects>> GetHomeProjects(int count)
        {
            var item = await _projectsService.GetHomeProject(count);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }


        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadPhotoAndSaveProject([FromForm] IFormFile file, [FromForm] Projects projects)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fotoğraf yüklenemedi.");

            // Fotoğrafı MinIO'ya yükle
            file = await CompressPhoto(file);
            var fileKey = await _minioFileService.UploadFileAsync(file);
            var photoUrl = _minioFileService.GetFileUrl(fileKey);
            projects.MainImageUrl = photoUrl;

            // Veritabanına kaydet
            await _projectsService.AddAsync(projects);

            return Ok(projects);
        }

        [HttpGet("ClearCache")]
        [Authorize]
        public IActionResult ClearCache()
        {
            _projectsService.ClearCache();
            return Ok();
        }

        [HttpGet("ClearCache/{slug}")]
        [Authorize]
        public IActionResult ClearCache(string slug)
        {
            _projectsService.ClearCache(slug);
            return Ok();
        }
        private async Task<IFormFile> CompressPhoto(IFormFile file)
        {
            // Orijinal fotoğrafı oku
            using (var image = await Image.LoadAsync<Rgba32>(file.OpenReadStream()))
            {
                // Yeniden boyutlandırma işlemi

                int targetWidth = 1920;
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(targetWidth, 0) // Yükseklik otomatik ayarlanır
                }));
                // JPEG formatında kalite ayarıyla kaydet
                var encoder = new JpegEncoder
                {
                    Quality = 75 // Kalite oranını ayarla (0-100 arası, 75 genelde iyi bir oran)
                };

                // Bellekte bir stream aç
                var memoryStream = new MemoryStream();
                await image.SaveAsync(memoryStream, encoder);
                memoryStream.Position = 0; // Stream başına dön

                // FormFile oluştururken içerik türünü manuel olarak kontrol et ve ayarla
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, file.Name, file.FileName)
                {
                    Headers = file.Headers,
                    ContentType = file.ContentType
                };

                // Eğer içerik türünü kontrol ediyorsanız burada yapılabilir:


                return formFile; // Bu formFile döndürülecek
            }
        }

    }
}
