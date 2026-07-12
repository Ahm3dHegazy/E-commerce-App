using CartFlow.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }

        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public Status OrderStatus { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string? StripePaymentIntentId { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
