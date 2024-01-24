using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Infrastructure.EntityFramework.Configurations
{
    public class registartiontConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            builder.HasKey(x => x.Id);

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<Registration> builder)
        {

            builder.HasOne(r => r.UserAccount)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.TicketType)
                .WithMany(t => t.Registrations)
                .HasForeignKey(r => r.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
