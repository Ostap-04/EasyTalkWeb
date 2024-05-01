using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class JobPostRepository : GenericRepository<JobPost>
    {
        private readonly AppDbContext _appDbContext;

        public JobPostRepository(AppDbContext appDbContext) : base(appDbContext) 
        {
            _appDbContext = appDbContext;
        }
       
        public  override async Task<IEnumerable<JobPost>> GetAllAsync()
        {
            return await _appDbContext.JobPosts
                .Include(j => j.Technologies)
                .ToListAsync();
        }

        public virtual async Task<JobPost> GetJobPostByIdForClient(Guid clientId, Guid jobPostId)
        {
            var client = await _appDbContext.Clients
                .Include(c => c.JobPosts)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client != null)
            {
                var jobPost = client.JobPosts.FirstOrDefault(j => j.Id == jobPostId);
                return jobPost;
            }

            return null!;
        }
    }
}
