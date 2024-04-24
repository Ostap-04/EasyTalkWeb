using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class FreelancerRepository : GenericRepository<Freelancer>
    {
        public FreelancerRepository(AppDbContext _context) : base(_context) { }

        public Freelancer GetFreelancerByPersonId(Guid personId)
        {
            var personWithFreelancer = _context.People
                .Include(p => p.Freelancer)
                .FirstOrDefault(p => p.Id == personId);

            var freelancer = personWithFreelancer?.Freelancer;

            return freelancer;
        }
        public async Task<IEnumerable<Freelancer>> GetAllAsyncWithPerson()
        {
            return await _context.Freelancers
                .Include(j => j.Person)
                .Include(p => p.Proposals)
                .ToListAsync();
        }
    }
}