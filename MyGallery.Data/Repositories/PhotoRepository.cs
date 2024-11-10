using Microsoft.EntityFrameworkCore;
using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Data.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly MyGalleryContext _context;

        public PhotoRepository(MyGalleryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Photo>> GetAllAsync()
        {
            return await _context.Photos.ToListAsync();
        }

        public async Task<Photo> GetByIdAsync(int id)
        {
            return await _context.Photos.FindAsync(id);
        }

        public async Task AddAsync(Photo item)
        {
            await _context.Photos.AddAsync(item);
            await _context.SaveChangesAsync();
        }
    }
}
