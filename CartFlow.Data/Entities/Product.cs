using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();

        public List<CartItem> CartItems { get; set; } = new();

        public List<Review> Reviews { get; set; } = new();

        public List<ProductImage> ProductImages { get; set; } = new();

        [NotMapped]
        public ProductImage PrimaryImage => ProductImages?.FirstOrDefault(img => img.IsPrimary)
            ?? ProductImages?.FirstOrDefault();
    }
}
