using CartFlow.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CartFlow.Data.Entities {
    public class User {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public Role UserRole { get; set; }
        public string Phone { get; set; }
        public List<Address> Addresses { get; set; } = new();
    }
}
