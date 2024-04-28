using EasyTalkWeb.Persistance.EntityConfigutation;
using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(JobPostConfiguration))]
    public class JobPost : BaseEntity
	{
		public string? Title {  get; set; }
		
		public decimal? Price { get; set; }
		
		public string? Description { get; set; }
		
		public Guid ClientId { get; set; }
		
		public Client Client { get; set; }
        
		public Project? Project { get; set; }

        public ICollection<Technology>? Technologies { get; set; }
		public ICollection<Proposal>? Proposals {  get; set; } 

    }
}
