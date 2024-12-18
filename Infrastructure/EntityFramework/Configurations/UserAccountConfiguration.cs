﻿using Domain.Entities;
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

            builder.HasOne(u => u.Role)
                .WithMany(r => r.UserAccounts)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Events)
                .WithOne(e => e.UserAccount)
                .HasForeignKey(e => e.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Reservations)
                .WithOne(r => r.UserAccount)
                .HasForeignKey(r => r.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notifications)
              .WithOne(n => n.UserAccount)
              .HasForeignKey(n => n.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Permissions)
               .WithMany(p => p.UserAccounts)
               .UsingEntity(j => j.ToTable("UserPermissions"));
        }
    }
}
