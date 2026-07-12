using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Descripion { get; set; }

        public List<Product> Products { get; set; } = new();

        public int? ParentCategoryId { get; set; }
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }

        public List<Category> Subcategories { get; set; } = new();
    }
}