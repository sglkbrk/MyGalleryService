using MyGallery.Data.Repositories;
using MyGallery.Domain;
using MyGallery.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Services
{
    public class ContantMeService : IContantMeService
    {
        private readonly IContantMeRepository _contantMeRepository;

        public ContantMeService(IContantMeRepository contantMeRepository)
        {
            _contantMeRepository = contantMeRepository;
        }

        public async Task<IEnumerable<ContantMe>> GetAllAsync()
        {
            return await _contantMeRepository.GetAllAsync();
        }

        public async Task<ContantMe> GetByIdAsync(int id)
        {
            return await _contantMeRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(ContantMe item)
        {
            await _contantMeRepository.AddAsync(item);
        }

    }
}