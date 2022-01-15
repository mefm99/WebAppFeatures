using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models
{
    [Table("RegisterVerification")]
    public class RegisterVerification
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string GeneratedToken { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public bool Status { get; set; }
        public bool VerificationStatus { get; set; }
        public DateTime? VerificationDate { get; set; }
    }
}
