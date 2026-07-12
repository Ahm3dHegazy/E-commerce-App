using System.ComponentModel.DataAnnotations.Schema;


namespace CartFlow.Data.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Governorate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        public int UserID { get; set; }
        [ForeignKey(nameof(UserID))]
        public User User { get; set; }
    }
}
