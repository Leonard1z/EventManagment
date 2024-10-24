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
    public class RoleConfiguration : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.HasKey(x => x.Id);
            RelationShips(builder);
        }

        private void RelationShips(EntityTypeBuilder<Roles> entityTypeBuilder)
        {
            entityTypeBuilder.HasMany(r => r.UserAccounts)
               .WithOne(u => u.Role)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

            entityTypeBuilder.HasMany(p => p.Permissions)
                .WithMany(r => r.Roles)
                .UsingEntity(j => j.ToTable("RolePermission")); 
        }
    }
}
