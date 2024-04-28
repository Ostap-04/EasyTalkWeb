
using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class TechRepository : GenericRepository<Technology>
    {
        private readonly AppDbContext _appDbContext;
       
        public TechRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Technology> GetTechnologyWithFreelancerByIdAsync(Guid techId)
        {
            var technology = await _appDbContext.Technologies
                .Include(c => c.Freelancers)
                .FirstOrDefaultAsync(c => c.Id == techId);

            return technology;
        }

        public async Task<Technology> GetTechnologyWithFreelancerByNameAsync(string name)
        {
            var technology = await _appDbContext.Technologies
                .Include(c => c.Freelancers)
                .FirstOrDefaultAsync(c => c.Name == name);

            return technology;
        }
    }
}
