using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations
{
    public class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.HasKey(x => x.Id);

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<UserAccount> builder)
        {

            builder.HasMany(u => u.Registrations)
            .WithOne(r => r.UserAccount)
            .HasForeignKey(r => r.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
