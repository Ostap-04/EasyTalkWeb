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
		
		public DbSet<Person> People { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Freelancer> Freelancers { get; set; }

		public DbSet<Technology> Technologies { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<Chatshot> Chatshots { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

		public DbSet<JobPost> JobPosts { get; set; }

        public DbSet<Proposal> Proposals { get; set; }
    }
}
