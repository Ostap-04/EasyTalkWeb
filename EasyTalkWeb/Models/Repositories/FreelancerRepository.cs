using EasyTalkWeb.Persistance;

namespace EasyTalkWeb.Models.Repositories
{
    public class FreelancerRepository: GenericRepository<Freelancer>
    {
        public FreelancerRepository(AppDbContext _context) : base(_context) { }
    }
}
