using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Data.Repositories
{
    public interface IProjectsRepository
    {
        Task<IEnumerable<Projects>> GetAllAsync();
        Task<Projects> GetByIdAsync(int id);
        Task AddAsync(Projects item);
        Task<Projects> GetProjectWithPhotosAsync(string slug);
        Task<IEnumerable<Projects>> GetRecentProject(int count);
        Task<IEnumerable<Projects>> GetHomeProject(int count);
        Task<Projects> GetProjectAllPhotosAsync(string slug, Format format);



    }
}
