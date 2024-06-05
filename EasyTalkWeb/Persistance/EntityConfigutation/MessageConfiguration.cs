using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .HasKey(m => m.Id);
            builder
                .Property(m => m.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(m => m.Text)
                .HasMaxLength(2000);
            builder
                .Property(m => m.CreatedDate)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .IsRequired();
            builder
                .HasOne(m => m.Person)
                .WithMany(p => p.Messages)
                .HasForeignKey(m => m.PersonId)
                .IsRequired();
            builder
                .HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId);
        }
    }
}
