using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Precision(18, 2)]
        public decimal Rate { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

    }
}