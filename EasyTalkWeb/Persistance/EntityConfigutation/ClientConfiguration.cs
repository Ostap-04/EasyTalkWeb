using EasyTalkWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        void IEntityTypeConfiguration<Client>.Configure(EntityTypeBuilder<Client> builder)
        {
            builder
                .HasKey(c => c.ClientId);
            builder
                .Property(a => a.ClientId)
				.ValueGeneratedOnAdd()
				.IsRequired();
            builder
               .Property(a => a.CreatedDate)
               .ValueGeneratedOnAdd()
               .IsRequired();
            builder
                .HasOne(c => c.Person)
                .WithOne(p => p.Client)
                .HasForeignKey<Client>(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            builder
                .HasMany(c => c.Projects)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId);
        }
    }
}
