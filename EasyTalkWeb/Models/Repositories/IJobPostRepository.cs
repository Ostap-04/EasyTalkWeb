namespace EasyTalkWeb.Models.Repositories
{
    public interface IJobPostRepository
    {
        Task<IEnumerable<JobPost>> GetAllAsync();
        Task<JobPost?> GetAsync(Guid id);
        Task<JobPost> AddAsync(JobPost jobpost);
        Task<JobPost?> UpdateAsync(JobPost jobpost);
        Task<JobPost?> DeleteAsync(Guid id);
    }
}
