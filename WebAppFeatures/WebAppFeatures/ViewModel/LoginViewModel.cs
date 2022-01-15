using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WebAppFeatures.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "UserName")]
        [Required(AllowEmptyStrings = false,ErrorMessage = "UserNameRequired")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
