using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models.DataModel
{
    public class DashBoardInfo
    {
        public User User { get; set; }
        public string NumViewers { get; set; }
        public RegisterVerification RegisterVerification { get; set; }
    }
}
