using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTalkWeb.Persistance.EntityConfigutation
{
	public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
	{
		public void Configure(EntityTypeBuilder<Proposal> builder)
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
				.Property(p => p.ModifiedDate);
			builder
				.Property(p => p.Title)
				.HasMaxLength(256);
			builder
				.Property(p => p.Text)
				.HasMaxLength(4096);
			builder
				.HasOne(p => p.Freelancer)
				.WithMany(c => c.Proposals)
				.HasForeignKey(p => p.FreelancerId)
				.IsRequired();
		}
	}
}