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
	}
}
