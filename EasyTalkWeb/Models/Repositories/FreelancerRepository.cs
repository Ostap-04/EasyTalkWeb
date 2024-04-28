using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class FreelancerRepository : GenericRepository<Freelancer>
    {
        public FreelancerRepository(AppDbContext _context) : base(_context) { }

        public async Task<Freelancer> GetFreelancerByPersonId(Guid personId)
        {
            var personWithFreelancer = await _context.People
                .Include(p => p.Freelancer)
                .FirstOrDefaultAsync(p => p.Id == personId);

            var freelancer = personWithFreelancer?.Freelancer;

            return freelancer;
        }

        public async Task<Person> GetPersonByFreelancerId(Guid freelancerId)
        {
            var freelancer = await _context.Freelancers
                .Include(f => f.Person)
                .FirstOrDefaultAsync(f => f.FreelancerId == freelancerId);

            var person = freelancer?.Person;

            return person;
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