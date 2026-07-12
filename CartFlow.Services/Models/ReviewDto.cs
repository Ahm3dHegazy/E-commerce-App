namespace CartFlow.Services.Models
{
    /// <summary>
    /// Data transfer model for reviews. Placed in the Services layer so Web can depend
    /// on Services without circular references.
    /// </summary>
    public class ReviewDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
