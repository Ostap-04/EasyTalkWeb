using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace EasyTalkWeb.Models.Repositories
{
    public class FreelancerRepository : GenericRepository<Freelancer>
    {
        public FreelancerRepository(AppDbContext _context) : base(_context) { }

        public virtual  Freelancer GetFreelancerByPersonId(Guid personId)
        {
            var personWithFreelancer =  _context.People
                .Include(p => p.Freelancer)
                .FirstOrDefault(p => p.Id == personId);

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

        public virtual async Task<IEnumerable<Freelancer>> GetAllAsyncWithPerson()
        {
            return await _context.Freelancers
                .Include(j => j.Person)
                .Include(p => p.Proposals)
                .Include(t=>t.Technologies)
                .ToListAsync();
        }
        public virtual async Task<IEnumerable<Freelancer>> GetFreelancersBySearch(string searchTerm)
        {
            // Construct the tsquery string
            //var tsQuery = $"to_tsquery('english', '{searchTerm}')";

            // Perform the search using raw SQL query
            //var freelancers = _context.Freelancers
            //    .FromSqlInterpolated($"SELECT * FROM public.\"Freelancers\" WHERE ts @@ to_tsquery('english', {searchTerm})")
            //    .ToList();

            var freelancers = _context.Freelancers
                .Include(f => f.Person)
                .Include (f => f.Proposals)
                .Where(p => EF.Functions.ToTsVector("english", p.Specialization)
                    .Matches(EF.Functions.ToTsQuery("english", searchTerm)))
                .ToList();


            return freelancers;
        }
    }
}