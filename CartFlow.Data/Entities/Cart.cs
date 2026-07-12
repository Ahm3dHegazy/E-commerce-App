using System.ComponentModel.DataAnnotations.Schema;

namespace CartFlow.Data.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public List<CartItem> CartItems { get; set; } = new();
    }
}
