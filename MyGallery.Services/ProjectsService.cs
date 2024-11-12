using MyGallery.Data.Repositories;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MyGallery.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly IMemoryCache _cache;
        private static List<string> _cacheKeys = new List<string>();

        public ProjectsService(IProjectsRepository projectsRepository, IMemoryCache cache)
        {
            _projectsRepository = projectsRepository;
            _cache = cache;
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
            ClearCache();
            await _projectsRepository.AddAsync(item);
        }

        public async Task<Projects> GetProjectWithPhotosAsync(string slug)
        {
            string cacheKey = $"PostProjects_{slug}";
            if (_cache.TryGetValue(cacheKey, out Projects projects))
            {
                return projects;
            }
            projects = await _projectsRepository.GetProjectWithPhotosAsync(slug);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(20)); // 1 gün boyunca cache'de sakla
            _cache.Set(cacheKey, projects, cacheOptions);
            return projects;
            // return await _projectsRepository.GetProjectWithPhotosAsync(slug);
        }

        public async Task<IEnumerable<Projects>> GetRecentProject(int count)
        {
            // return await _projectsRepository.GetRecentProject(count);
            string cacheKey = $"RecentProjects_{count}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Projects> projects))
            {
                return projects;
            }
            projects = await _projectsRepository.GetRecentProject(count);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(20)); // 1 gün boyunca cache'de sakla
            _cache.Set(cacheKey, projects, cacheOptions);
            _cacheKeys.Add(cacheKey);
            return projects;
        }

        public async Task<IEnumerable<Projects>> GetHomeProject(int count)
        {
            // Cache anahtarı, "homeProjects_" ile `count` değeriyle birleştirilerek oluşturulur.
            string cacheKey = $"homeProjects_{count}";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Projects> projects))
            {
                return projects;
            }
            projects = await _projectsRepository.GetHomeProject(count);
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(20)); // 1 gün boyunca cache'de sakla
            _cache.Set(cacheKey, projects, cacheOptions);
            _cacheKeys.Add(cacheKey);
            return projects;
        }

        public async Task<Projects> GetProjectAllPhotosAsync(string slug, Format format)
        {
            return await _projectsRepository.GetProjectAllPhotosAsync(slug, format);
        }

        public async Task<List<string>> GetAllSlugsAsync()
        {
            string cacheKey = $"Projects_slugs";
            if (_cache.TryGetValue(cacheKey, out List<string> slugs))
            {
                return slugs;
            }
            slugs = await _projectsRepository.GetAllSlugsAsync();
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(20)); // 1 gün boyunca cache'de sakla
            _cache.Set(cacheKey, slugs, cacheOptions);
            _cacheKeys.Add(cacheKey);
            return slugs;

        }
        public void ClearCache()
        {
            foreach (var key in _cacheKeys)
            {
                _cache.Remove(key); // Her bir anahtarı temizliyoruz
            }
            _cacheKeys.Clear(); // Anahtarları listeyi sıfırlıyoruz
        }
        public void ClearCache(string slug)
        {
            string cacheKey = $"PostProjects_{slug}";
            _cache.Remove(cacheKey);
        }



    }
}