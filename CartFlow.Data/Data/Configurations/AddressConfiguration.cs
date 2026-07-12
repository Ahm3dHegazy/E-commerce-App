using CartFlow.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartFlow.Data.Data.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Governorate)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.City)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Street)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany(e => e.Addresses)
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Addresses");
        }
    }
}

