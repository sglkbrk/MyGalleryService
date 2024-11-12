using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services.Interfaces
{
    public interface IProjectsService
    {
        Task<IEnumerable<Projects>> GetAllAsync();
        Task<Projects> GetByIdAsync(int id);
        Task AddAsync(Projects item);
        Task<Projects> GetProjectWithPhotosAsync(string slug);

        Task<IEnumerable<Projects>> GetRecentProject(int count);
        Task<IEnumerable<Projects>> GetHomeProject(int count);
        Task<Projects> GetProjectAllPhotosAsync(string slug, Format format);
        void ClearCache();
        void ClearCache(string slug);
    }
}
