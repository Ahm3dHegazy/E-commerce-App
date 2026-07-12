using CartFlow.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartFlow.Data.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(e => e.Password)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.UserRole)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Phone)
                .HasMaxLength(11);

            builder.ToTable("Users");
        }
    }
}

