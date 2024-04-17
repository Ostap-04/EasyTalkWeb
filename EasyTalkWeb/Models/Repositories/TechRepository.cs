
using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class TechRepository : ITechRepository
    {
        private readonly AppDbContext appDbContext;

        public TechRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<IEnumerable<Technology>> GetAllAsync()
        {
            return await appDbContext.Technologies.ToListAsync();
        }

        public async Task<Technology?> GetAsync(Guid id)
        {
            return await appDbContext.Technologies.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
