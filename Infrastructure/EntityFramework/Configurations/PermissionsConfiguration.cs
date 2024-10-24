using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityFramework.Configurations
{
    public class PermissionsConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(x => x.Id);

            RelationShips(entityTypeBuilder);
        }

        private void RelationShips(EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.HasMany(p => p.Roles)
                .WithMany(r => r.Permissions)
                .UsingEntity(j => j.ToTable("RolePermission"));

            entityTypeBuilder.HasMany(p => p.UserAccounts)
               .WithMany(u => u.Permissions)
               .UsingEntity(j => j.ToTable("UserPermissions"));
        }

    }
}
