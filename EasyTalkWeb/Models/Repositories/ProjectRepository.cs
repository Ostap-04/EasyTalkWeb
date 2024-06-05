using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class ProjectRepository: GenericRepository<Project>
    {
        public ProjectRepository(AppDbContext _context) : base(_context) { }

        public virtual async Task<List<Project>> GetAllProjects()
        {
            var projects = _context.Projects
                .Include(p => p.Freelancer)
                .ThenInclude(f => f.Person)
                .Include(p => p.Client)
                .ThenInclude(c => c.Person)
                .ToListAsync();
            return await projects;
        }
    }
}
