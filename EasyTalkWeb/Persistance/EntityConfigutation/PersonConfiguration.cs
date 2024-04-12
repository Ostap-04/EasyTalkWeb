using EasyTalkWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.EntityConfiguration
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder
                .HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder
                .Property(p => p.FirstName);
            builder
                .Property(p => p.LastName);
            builder
                .Property(p => p.DateOfBirth);
            builder
                .Property(p => p.Email)
                .IsRequired();
            builder
                .Property(p => p.CreatedDate)
                .ValueGeneratedOnAdd();
            builder
                .Property(p => p.Gender);
            builder
                .Property(p => p.ModifiedDate);
            builder
                .Property(p => p.PhotoLocation);
            builder
                .Property(p => p.Location);
            builder
                .HasOne(p => p.Client)
                .WithOne(p => p.Person)
                .HasForeignKey<Client>(c => c.PersonId)
                .IsRequired(false);
            builder
                .HasOne(p => p.Freelancer)
                .WithOne(p => p.Person)
                .HasForeignKey<Freelancer>(f => f.PersonId)
                .IsRequired(false);
            builder
                .HasMany(p => p.Chats)
                .WithMany(c => c.Persons);
            builder
                .HasMany(p => p.Messages)
                .WithOne(m => m.Person)
                .HasForeignKey(m => m.PersonId);
            // identity https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-8.0
            //builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            //// Limit the size of columns to use efficient database types
            //builder.Property(u => u.UserName).HasMaxLength(256);
            //builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
            //builder.Property(u => u.Email).HasMaxLength(256);
            //builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
            //builder.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

            //// Each User can have many UserLogins
            //builder.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

            //// Each User can have many UserTokens
            //builder.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

            //// Each User can have many entries in the UserRole join table
            //builder.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        }
    }
}
