using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(ProjectConfiguration))]
    public class Project : BaseEntity
    {

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; }

        public Guid ClientId { get; set; }

        public Guid FreelancerId { get; set; }

        public Client? Client { get; set; }

        public Freelancer? Freelancer { get; set; }

        public ICollection<Topic>? Topics { get; set; }
    }
}
