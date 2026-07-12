using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
