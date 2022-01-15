using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("Id")]
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneKey { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Gender { get; set; }
        public byte[] Image { get; set; }
        public bool Status { get; set; }
        public bool Deleted { get; set; }
    }
}
