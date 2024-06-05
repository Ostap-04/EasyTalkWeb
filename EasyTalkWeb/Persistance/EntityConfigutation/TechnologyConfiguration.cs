using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class TechnologyConfiguration : IEntityTypeConfiguration<Technology>
    {
        public void Configure(EntityTypeBuilder<Technology> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(p => p.Name);
            builder
                .HasMany(t => t.Freelancers)
                .WithMany(f => f.Technologies);
            builder
                .HasMany(t => t.Jobposts)
                .WithMany(f => f.Technologies);
            builder
                .HasMany(t => t.Proposals)
                .WithMany(f => f.Technologies);
        }
    }
}
