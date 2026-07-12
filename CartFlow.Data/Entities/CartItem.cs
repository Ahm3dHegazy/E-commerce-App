using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
