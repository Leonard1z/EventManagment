using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityFramework.Configurations
{
    public class ReservationConfiguration:IEntityTypeConfiguration<Reservation>
    {

        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasKey(x => x.Id);

            RelationShips(builder);
        }

        private void RelationShips(EntityTypeBuilder<Reservation> builder)
        {
            builder.HasOne(r=>r.TicketTypes)
                .WithMany(t=>t.Reservations)
                .HasForeignKey(r=>r.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.UserAccount)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
