using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityFramework.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            Relationships(builder);
        }

        private void Relationships(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(c => c.Events)
           .WithOne(e => e.Category)
           .HasForeignKey(e => e.CategoryId)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
