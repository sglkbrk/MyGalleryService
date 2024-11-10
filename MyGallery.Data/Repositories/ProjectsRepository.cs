using Microsoft.EntityFrameworkCore;
using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Data.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly MyGalleryContext _context;

        public ProjectsRepository(MyGalleryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Projects>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<Projects> GetByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<Projects> GetProjectWithPhotosAsync(string slug)
        {
            var project = await _context.Projects
            .Where(p => p.Slug == slug)
            .FirstOrDefaultAsync();

            if (project != null)
            {
                // Format 0 olan ilk 5 fotoğraf
                var photosFormat0 = await _context.Photos
                    .Where(photo => photo.ProjectsId == project.Id && photo.Format == Format.horizontal)
                    .Take(6)
                    .ToListAsync();

                // Format 1 olan ilk 5 fotoğraf
                var photosFormat1 = await _context.Photos
                    .Where(photo => photo.ProjectsId == project.Id && photo.Format == Format.vertical)
                    .Take(4)
                    .ToListAsync();

                // İki listeyi birleştir
                project.Photos = photosFormat0.Concat(photosFormat1).ToList();
            }

            return project;
        }

        public async Task<Projects> GetProjectAllPhotosAsync(string slug, Format format)
        {
            var project = await _context.Projects
            .Where(p => p.Slug == slug)
            .FirstOrDefaultAsync();
            if (project != null)
            {
                project.Photos = await _context.Photos
                    .Where(photo => photo.ProjectsId == project.Id && photo.Format == format)
                    .Take(5)
                    .ToListAsync();
            }
            return project;
        }
        public async Task<IEnumerable<Projects>> GetRecentProject(int count)
        {
            return await _context.Projects
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projects>> GetHomeProject(int count)
        {
            return await _context.Projects.Where(p => p.HomePage == true)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task AddAsync(Projects item)
        {
            await _context.Projects.AddAsync(item);
            await _context.SaveChangesAsync();
        }
    }
}
