using CartFlow.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CartFlow.Data.Entities {
    public class Order {
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
    }
}
