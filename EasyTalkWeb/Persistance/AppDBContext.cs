using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EasyTalkWeb.Persistance
{
    public class AppDbContext : IdentityDbContext<Person, UserRole, Guid>
	{
		public AppDbContext() : base() { }

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}
		
		DbSet<Person> People { get; set; }

		DbSet<Client> Clients { get; set; }

		DbSet<Freelancer> Freelancers { get; set; }

		DbSet<Technology> Technologies { get; set; }

		DbSet<Project> Projects { get; set; }

		DbSet<Topic> Topics { get; set; }

		DbSet<Chatshot> Chatshots { get; set; }

		DbSet<Chat> Chats { get; set; }

		DbSet<Message> Messages { get; set; }

		DbSet<Attachment> Attachments { get; set; }

		DbSet<JobPost> JobPosts { get; set; }
		
		DbSet<Proposal> Proposals { get; set; }
    }
}
