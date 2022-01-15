using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.Models
{
    public class AppSettings
    {
        public string EmailFrom { get; set; }
        public string Port { get; set; }
        public string Host { get; set; }
        public string Password { get; set; }
        public string UrlSearch { get; set; }
        public string VerifyRegistrationUrl { get; set; }
        public string PasswordResetURL { get; set; }
        public string VerifyResetPasswordUrl { get; set; }
        public string UrlIndexSearch { get; set; }
    }
}
