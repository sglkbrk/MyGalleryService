using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services.Interfaces
{
    public interface IContantMeService
    {
        Task<IEnumerable<ContantMe>> GetAllAsync();
        Task<ContantMe> GetByIdAsync(int id);
        Task AddAsync(ContantMe item);


    }
}
