using EasyTalkWeb.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.EntityConfiguration;

namespace EasyTalkWeb.Models
{
    [EntityTypeConfiguration(typeof(PersonConfiguration))]
    public class Person : IdentityUser<Guid>
    {
        public Gender? Gender { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Location { get; set; }

        public string? PhotoLocation { get; set; }
        
        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Client? Client { get; set; }

        public Freelancer? Freelancer { get; set; }

        public ICollection<Chat>? Chats { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
