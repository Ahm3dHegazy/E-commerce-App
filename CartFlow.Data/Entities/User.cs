using CartFlow.Data.Enums;
using CartFlow.Data.Validation;
using System.ComponentModel.DataAnnotations;

namespace CartFlow.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public Role UserRole { get; set; }
        public string Phone { get; set; } = string.Empty;
        public List<Address> Addresses { get; set; } = new();
    }
}
