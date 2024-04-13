using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class FreelancerConfiguration : IEntityTypeConfiguration<Freelancer>
    {
        public void Configure(EntityTypeBuilder<Freelancer> builder)
        {
            builder
                .HasKey(f => f.FreelancerId);
            builder
                .Property(f => f.FreelancerId)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(f => f.CreatedDate)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(f => f.ModifiedDate);
            builder
                .Property(f => f.Specialization);
            builder
                .Property(f => f.Rate);
            builder
                .HasOne(f => f.Person)
                .WithOne(p => p.Freelancer)
                .HasForeignKey<Freelancer>(f => f.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            builder
                .HasMany(f => f.Projects)
                .WithMany(p => p.Freelancers);
            builder
                .HasMany(f => f.Technologies)
                .WithMany(t => t.Freelancers);
            builder
                .HasMany(f => f.Proposals)
                .WithOne(p => p.Freelancer)
                .HasForeignKey(p => p.FreelancerId);
        }
    }
}
