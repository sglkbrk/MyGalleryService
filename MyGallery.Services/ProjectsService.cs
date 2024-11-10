using MyGallery.Data.Repositories;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _projectsRepository;

        public ProjectsService(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task<IEnumerable<Projects>> GetAllAsync()
        {
            return await _projectsRepository.GetAllAsync();
        }

        public async Task<Projects> GetByIdAsync(int id)
        {
            return await _projectsRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Projects item)
        {
            await _projectsRepository.AddAsync(item);
        }

        public async Task<Projects> GetProjectWithPhotosAsync(string slug)
        {
            return await _projectsRepository.GetProjectWithPhotosAsync(slug);
        }

        public async Task<IEnumerable<Projects>> GetRecentProject(int count)
        {
            return await _projectsRepository.GetRecentProject(count);
        }

        public async Task<IEnumerable<Projects>> GetHomeProject(int count)
        {
            return await _projectsRepository.GetHomeProject(count);
        }

        public async Task<Projects> GetProjectAllPhotosAsync(string slug, Format format)
        {
            return await _projectsRepository.GetProjectAllPhotosAsync(slug, format);
        }
    }
}