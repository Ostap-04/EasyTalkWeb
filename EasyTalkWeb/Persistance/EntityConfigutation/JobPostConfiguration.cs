using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalkWeb.Persistance.EntityConfigutation
{
	public class JobPostConfiguration : IEntityTypeConfiguration<JobPost>
	{
		public void Configure(EntityTypeBuilder<JobPost> builder)
		{
			builder
				.HasKey(p => p.Id);
			builder
				.Property(p => p.Id)
				.ValueGeneratedOnAdd()
				.IsRequired();
			builder
				.Property(p => p.CreatedDate)
				.ValueGeneratedOnAdd()
				.IsRequired();
			builder
				.Property(p => p.Title)
				.HasMaxLength(256);
			builder
				.Property(p => p.Description)
				.HasMaxLength(4096);
			builder
			.Property(p => p.Price);
			builder
				.HasOne(p => p.Client)
				.WithMany(c => c.JobPosts)
				.HasForeignKey(p => p.ClientId)
				.IsRequired();
            builder
              .HasMany(f => f.Technologies)
              .WithMany(t => t.Jobposts);
			builder
				.HasOne(j => j.Project)
				.WithOne(p => p.JobPost);
			builder
				.HasMany(p => p.Proposals)
				.WithOne(j => j.JobPost)
				.HasForeignKey(f => f.JobPostId);
			   
           

        }
	}
}