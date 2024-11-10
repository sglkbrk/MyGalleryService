using MyGallery.Data.Repositories;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<IEnumerable<Photo>> GetAllAsync()
        {
            return await _photoRepository.GetAllAsync();
        }

        public async Task<Photo> GetByIdAsync(int id)
        {
            return await _photoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Photo item)
        {
            await _photoRepository.AddAsync(item);
        }

    }
}