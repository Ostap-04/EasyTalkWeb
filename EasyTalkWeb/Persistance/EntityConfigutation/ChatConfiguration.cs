using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder
                .HasKey(c => c.Id);
            builder
                .Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(c => c.Name)
                .HasMaxLength(50);
            builder
                .Property(c => c.Description)
                .HasMaxLength(200);
            builder
                .Property(a => a.CreatedDate)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(a => a.ModifiedDate);
            builder
                .HasMany(c => c.Persons)
                .WithMany(c => c.Chats);
            builder
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId);
        }
    }
}
