

using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class JobPostRepository : GenericRepository<JobPost>
    {
        private readonly AppDbContext appDbContext;

        public JobPostRepository(AppDbContext appDbContext) : base(appDbContext) { }
       
        //public async Task<JobPost> AddAsync(JobPost jobpost)
        //{
        //    await appDbContext.AddAsync(jobpost);
        //    //await appDbContext.SaveChangesAsync();
        //    return jobpost;
        //}

        //public async Task<JobPost?> DeleteAsync(Guid id)
        //{
        //    var existingPost = await appDbContext.JobPosts.FindAsync(id);
        //    if (existingPost != null)
        //    {
        //        appDbContext.JobPosts.Remove(existingPost);
        //        await appDbContext.SaveChangesAsync();
        //        return existingPost;
        //    }
        //    return null;
        //}

        //public async Task<IEnumerable<JobPost>> GetAllAsync()
        //{
        //    return await appDbContext.JobPosts.Include(x=>x.Technologies).ToListAsync();
        //}

        //public async Task<JobPost?> GetAsync(Guid id)
        //{
        //    return await appDbContext.JobPosts.FirstOrDefaultAsync(x => x.Id == id);
        //}

        //public async Task<JobPost?> UpdateAsync(JobPost jobpost)
        //{
        //    var existingPost = await appDbContext.JobPosts.FirstOrDefaultAsync(x => x.Id == jobpost.Id);
        //    if (existingPost != null)
        //    {
        //        existingPost.Title = jobpost.Title;
        //        existingPost.Description = jobpost.Description;
        //        existingPost.Price = jobpost.Price;
        //        existingPost.ModifiedDate = DateTime.UtcNow;
        //        existingPost.CreatedDate = jobpost.CreatedDate;
        //        existingPost.ClientId = jobpost.ClientId;
        //        existingPost.Client=jobpost.Client;

        //        await appDbContext.SaveChangesAsync();
        //        return existingPost;

        //    }
        //    return null;
        //}
    }
}
