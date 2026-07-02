using CartFlow.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartFlow.Data.Data.Configurations {
    public class OrderConfiguration : IEntityTypeConfiguration<Order> {
        public void Configure(EntityTypeBuilder<Order> builder) {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.PaymentMethod)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(o => o.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.StripePaymentIntentId)
                .HasMaxLength(255);

            builder.Property(o => o.AddressLine1)
                .HasMaxLength(200);

            builder.Property(o => o.City)
                .HasMaxLength(100);

            builder.Property(o => o.State)
                .HasMaxLength(100);

            builder.Property(o => o.PostalCode)
                .HasMaxLength(20);

            builder.Property(o => o.Country)
                .HasMaxLength(100);

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Orders");
        }
    }
}
