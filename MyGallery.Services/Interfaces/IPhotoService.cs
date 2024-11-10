using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<IEnumerable<Photo>> GetAllAsync();
        Task<Photo> GetByIdAsync(int id);
        Task AddAsync(Photo item);


    }
}
