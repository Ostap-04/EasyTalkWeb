using EasyTalkWeb.Persistance.EntityConfigutation;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(ProposalConfiguration))]
    public class Proposal : BaseEntity
	{
		public string Title { get; set; }
		public string Text { get; set; }
		public Guid FreelancerId { get; set; }
		public Freelancer Freelancer { get; set; }
        public ICollection<Technology>? Technologies { get; set; }
		public JobPost JobPost { get; set; }
		public Guid JobPostId { get; set; }
    }
}
