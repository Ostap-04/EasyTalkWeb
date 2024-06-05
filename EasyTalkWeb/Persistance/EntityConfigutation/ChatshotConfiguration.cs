using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class ChatshotConfiguration : IEntityTypeConfiguration<Chatshot>
    {
        public void Configure(EntityTypeBuilder<Chatshot> builder)
        {
            builder
               .HasKey(c => c.Id);
            builder
                .Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(a => a.CreatedDate)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .HasOne(c => c.Topic)
                .WithMany(t => t.Chatshots)
                .HasForeignKey(c => c.TopicId)
                .IsRequired();
        }
    }
}
