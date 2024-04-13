using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(TopicConfiguration))]
    public class Topic : BaseEntity
    {
        public string? Name { get; set; }

        public Guid ProjectId { get; set; }

        public Project? Project { get; set; }

        public ICollection<Chatshot>? Chatshots { get; set; }
    }
}
