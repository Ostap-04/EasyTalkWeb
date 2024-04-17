using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EasyTalkWeb.Models.Repositories
{
    public class PersonRepository : GenericRepository<Person>
    {
        private readonly AppDbContext _appDbContext;

        public PersonRepository(AppDbContext appDbContext) : base(appDbContext) 
        { 
            _appDbContext = appDbContext;
        }

        public async Task<Freelancer> GetPersonWithTechnologies(Guid userId)
        {
            var freelancer = await _appDbContext.Freelancers
                .Include(c => c.Technologies)
                .FirstOrDefaultAsync(c => c.PersonId == userId);

            return freelancer!;
        }

        public Freelancer GetFreelancerByPersonId(Guid personId)
        {
            var personWithFreelancer = _context.People
                .Include(p => p.Freelancer)
                .FirstOrDefault(p => p.Id == personId);

            var freelancer = personWithFreelancer?.Freelancer;

            return freelancer;
        }
    }
}
