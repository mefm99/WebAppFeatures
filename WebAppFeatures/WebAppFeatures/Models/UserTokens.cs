﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models
{
    [Table("UserTokens")]
    public class UserTokens
    {
        [Key]
        public int Id { get; set; }
        public string PasswordSalt { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
