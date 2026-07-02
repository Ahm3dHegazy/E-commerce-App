using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities {
    public class ProductImage {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string Image { get; set; }
        public bool IsPrimary { get; set; }
    }
}