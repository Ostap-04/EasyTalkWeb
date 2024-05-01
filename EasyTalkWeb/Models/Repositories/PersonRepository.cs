using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EasyTalkWeb.Models.Repositories
{
    public class PersonRepository : GenericRepository<Person>
    {
        public PersonRepository(AppDbContext _context) : base(_context) {}

        public virtual async Task<Freelancer> GetFreelancer(Guid userId)
        {
            var freelancer = await _context.Freelancers
                .Include(c => c.Technologies)
                .FirstOrDefaultAsync(c => c.PersonId == userId);

            return freelancer!;
        }

        public virtual async Task<Person> GetPersonWithTechnologiesById(Guid userId)
        {
            var person = await _context.People
            .Include(p => p.Freelancer)
            .ThenInclude(f => f.Technologies)
            .FirstOrDefaultAsync(p => p.Id == userId);

            return person;
        }

        public async Task<Freelancer> GetFreelancerWithTechnologiesByPersonId(Guid personId)
        {
            var personWithFreelancer = await _context.People
                .Include(p => p.Freelancer)
                .ThenInclude(f => f.Technologies)
                .FirstOrDefaultAsync(p => p.Id == personId);

            var freelancer = personWithFreelancer?.Freelancer;

            return freelancer;
        }
    }
}
