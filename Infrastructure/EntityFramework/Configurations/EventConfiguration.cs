using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(x => x.Id);

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<Event> builder)
        {
            builder.HasMany(x => x.Registrations)
                 .WithOne(x => x.Event)
                 .HasForeignKey(x => x.EventId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Events)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserAccount)
              .WithMany(x => x.Events)
              .HasForeignKey(x => x.UserAccountId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TicketTypes)
                .WithOne(x => x.Event)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
