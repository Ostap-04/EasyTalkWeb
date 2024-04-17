using EasyTalkWeb.Persistance;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models.Repositories
{
    public class ProposalRepository :
   GenericRepository<Proposal>
    {
        private readonly AppDbContext _appDbContext;

        public ProposalRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public override async Task<IEnumerable<Proposal>> GetAllAsync()
        {
            return await _appDbContext.Proposals
                .Include(j => j.Technologies)
                .ToListAsync();
        }
        public async Task<Proposal> GetProposaltByIdForFreelancer(Guid freelancerId, Guid proposalId)
        {
            var freelancer = await _appDbContext.Freelancers
                .Include(c => c.Proposals)
                .FirstOrDefaultAsync(c => c.FreelancerId == freelancerId);

            if (freelancer != null)
            {
                var proposal = freelancer.Proposals.FirstOrDefault(j => j.Id == proposalId);
                return proposal;
            }

            return null!;
        }

    }
}
