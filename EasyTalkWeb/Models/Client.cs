using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(ClientConfiguration))]
    public class Client
    {
		public Guid ClientId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
		
        public Guid? PersonId { get; set; }
        
        public Person? Person { get; set; }

        public ICollection<Project>? Projects { get; set; }

        public ICollection<JobPost> JobPosts { get; set; }
    }
}
