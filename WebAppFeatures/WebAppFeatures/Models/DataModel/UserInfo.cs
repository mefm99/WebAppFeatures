using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models.DataModel
{
    public class UserInfo
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
