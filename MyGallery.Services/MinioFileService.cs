using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyGallery.Services.Interfaces;

namespace MyGallery.Application.Services
{
    public class MinioFileService : IMinioFileService
    {
        private readonly string _rootPath;

        public MinioFileService(IConfiguration configuration)
        {
            // Proje dizini içinde uploads klasörünü kullanın
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Klasör mevcut değilse oluştur
            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_rootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<Stream> GetFileAsync(string fileName)
        {
            var filePath = Path.Combine(_rootPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", fileName);
            }

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        public string GetFileUrl(string fileName)
        {
            return fileName;
        }
    }
}
