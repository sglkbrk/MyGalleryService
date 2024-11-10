using MyGallery.Domain;
using Microsoft.AspNetCore.Http;

namespace MyGallery.Services.Interfaces
{
    public interface IMinioFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<Stream> GetFileAsync(string key);
        string GetFileUrl(string key);


    }
}
