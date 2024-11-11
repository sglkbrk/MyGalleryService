using Microsoft.EntityFrameworkCore;
using MyGallery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyGallery.Data.Repositories
{
    public class ContantMeRepository : IContantMeRepository
    {
        private readonly MyGalleryContext _context;

        public ContantMeRepository(MyGalleryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContantMe>> GetAllAsync()
        {
            return await _context.ContantMe.ToListAsync();
        }

        public async Task<ContantMe> GetByIdAsync(int id)
        {
            return await _context.ContantMe.FindAsync(id);
        }

        public async Task AddAsync(ContantMe item)
        {
            await _context.ContantMe.AddAsync(item);
            await _context.SaveChangesAsync();
        }
    }
}
