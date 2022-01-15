using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models.DataModel
{
    public class UserInfoSearch
    {
        public string Key { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneKey { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Gender { get; set; }
        public byte[] Image { get; set; }
    }
}
