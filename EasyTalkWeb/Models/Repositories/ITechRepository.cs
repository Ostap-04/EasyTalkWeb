namespace EasyTalkWeb.Models.Repositories
{
    public interface ITechRepository
    {
        Task<IEnumerable<Technology>> GetAllAsync();
        Task<Technology?> GetAsync(Guid id);
        //Task<JobPost> AddAsync(JobPost jobpost);
        //Task<JobPost?> UpdateAsync(JobPost jobpost);
        //Task<JobPost?> DeleteAsync(Guid id);
    }
}
