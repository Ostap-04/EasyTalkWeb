using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(ChatConfiguration))]
    public class Chat : BaseEntity
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public Project? Project { get; set; }

        public ICollection<Message>? Messages { get; set; }

        public ICollection<Person>? Persons { get; set; }
    }
}
