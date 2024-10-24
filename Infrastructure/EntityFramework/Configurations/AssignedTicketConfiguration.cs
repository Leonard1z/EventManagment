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
    public class AssignedTicketConfiguration : IEntityTypeConfiguration<AssignedTicket>
    {
        public void Configure(EntityTypeBuilder<AssignedTicket> builder)
        {
            builder.HasKey(x => x.Id);

            RelationShips(builder);
            
        }

        private void RelationShips(EntityTypeBuilder<AssignedTicket> builder)
        {
            builder.HasOne(a=>a.Registration)
                .WithMany(r=>r.AssignedTickets)
                .HasForeignKey(a=>a.RegistrationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
