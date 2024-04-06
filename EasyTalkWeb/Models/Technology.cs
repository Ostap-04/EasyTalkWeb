using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(TechnologyConfiguration))]
    public class Technology
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Freelancer>? Freelancers { get; set; }

    }
}
