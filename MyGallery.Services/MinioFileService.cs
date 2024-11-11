using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MyGallery.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;





namespace MyGallery.Application.Services
{
    public class MinioFileService : IMinioFileService
    {
        private readonly string _rootPath;
        private readonly string _fontPath;

        public MinioFileService(IConfiguration configuration)
        {
            // Proje dizini içinde uploads klasörünü kullanın
            _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            _fontPath = Path.Combine(Directory.GetCurrentDirectory(), "fonts");

            // Klasör mevcut değilse oluştur
            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }
        }

        // public async Task<string> UploadFileAsync(IFormFile file)
        // {
        //     var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //     var filePath = Path.Combine(_rootPath, fileName);

        //     using (var stream = new FileStream(filePath, FileMode.Create))
        //     {
        //         await file.CopyToAsync(stream);
        //     }

        //     return fileName;
        // }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // Yüklenecek dosyanın adı ve yolu
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_rootPath, fileName);

            if (Path.GetExtension(file.FileName).ToLower() == ".jpg" || Path.GetExtension(file.FileName).ToLower() == ".png" || Path.GetExtension(file.FileName).ToLower() == ".jpeg")
            {
                using (var image = await Image.LoadAsync(file.OpenReadStream()))
                {
                    // Font dosyasını yükleyin (örneğin "Arial.ttf" dosyasını projeye eklediniz)
                    var fontPath = Path.Combine(_fontPath, "Arial.ttf"); // Font dosyasının yolu
                    var fontCollection = new FontCollection();
                    var fontFamily = fontCollection.Add(fontPath);
                    var font = fontFamily.CreateFont(22); // Yazı tipi ve boyutu
                    // var font = SystemFonts.CreateFont("DejaVu Sans", 20);
                    var color = Color.Gray; // Kırmızı renk
                    float horizontalSpacing = image.Width / 5; // Yazılar arasındaki yatay mesafe
                    float verticalSpacing = image.Height / 5; // Yazılar arasındaki dikey mesafe
                    float startX = image.Width / 5 / 2; // Yazıların başlangıç X konumu
                    float startY = image.Width / 5 / 2;
                    int counter = 1; // Yazı numarası (1'den 9'a kadar)
                    for (int row = 0; row < 5; row++) // 3 satır
                    {
                        for (int col = 0; col < 5; col++) // 3 sütun
                        {
                            var position = new PointF(startX + col * horizontalSpacing, startY + row * verticalSpacing);
                            image.Mutate(ctx => { ctx.DrawText("Bsgallery", font, color, position); });
                            counter++;
                        }
                    }
                    await image.SaveAsync(filePath);
                }
            }
            else
            {
                // Resim değilse, dosyayı olduğu gibi kaydedin
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
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
