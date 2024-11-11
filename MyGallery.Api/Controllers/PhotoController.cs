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
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IMinioFileService _minioFileService;

        public PhotoController(IPhotoService photoService, IMinioFileService minioFileService)
        {
            _photoService = photoService;
            _minioFileService = minioFileService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosItems()
        {
            return Ok(await _photoService.GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhotosItem(Photo item)
        {
            await _photoService.AddAsync(item);
            return CreatedAtAction(nameof(GetPhotosItems), new { id = item.Id }, item);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhotosItem(int id)
        {
            var item = await _photoService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file, [FromForm] Photo photo)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fotoğraf yüklenemedi.");

            // Fotoğrafı MinIO'ya yükle
            file = await CompressPhoto(file);
            var fileKey = await _minioFileService.UploadFileAsync(file);
            var photoUrl = _minioFileService.GetFileUrl(fileKey);
            photo.PhotoUrl = photoUrl;

            // EXIF metadata bilgilerini al
            var metadata = ImageMetadataReader.ReadMetadata(file.OpenReadStream());
            var subIfdDirectory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();

            if (subIfdDirectory != null)
            {
                photo.Camera = subIfdDirectory.GetDescription(ExifDirectoryBase.TagModel) ?? "Bilinmiyor";
                photo.Lens = subIfdDirectory.GetDescription(ExifDirectoryBase.TagLensModel) ?? "Bilinmiyor";
                photo.FocalLength = subIfdDirectory.GetDescription(ExifDirectoryBase.TagFocalLength) ?? "Bilinmiyor";
                photo.Aperture = subIfdDirectory.GetDescription(ExifDirectoryBase.TagAperture) ?? "Bilinmiyor";
                photo.Iso = subIfdDirectory.GetDescription(ExifDirectoryBase.TagIsoEquivalent) ?? "Bilinmiyor";
                photo.ShutterSpeed = subIfdDirectory.GetDescription(ExifDirectoryBase.TagShutterSpeed) ?? "Bilinmiyor";
                photo.Date = subIfdDirectory.GetDescription(ExifDirectoryBase.TagDateTimeOriginal) ?? "Bilinmiyor";
            }
            using (var image = await Image.LoadAsync<Rgba32>(file.OpenReadStream()))
            {
                photo.Width = image.Width;
                photo.Height = image.Height;
                photo.Format = image.Width > image.Height ? Format.horizontal : Format.vertical;
            }

            // Diğer bilgileri doldur
            photo.Size = (int)file.Length;

            // Veritabanına kaydet
            await _photoService.AddAsync(photo);

            return Ok(photo);
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
