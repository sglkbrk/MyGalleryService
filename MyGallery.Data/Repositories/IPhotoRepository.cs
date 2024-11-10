using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Data.Repositories
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetAllAsync();
        Task<Photo> GetByIdAsync(int id);
        Task AddAsync(Photo item);
    }
}
