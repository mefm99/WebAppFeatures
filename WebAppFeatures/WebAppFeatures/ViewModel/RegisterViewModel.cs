using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "PasswordRequired")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,16}$", ErrorMessage = "PasswordRegularExpression")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "ComparePassword")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "ComparePasswordCompare")]
        [Required(ErrorMessage = "ComparePasswordRequired")]
        public string ComparePassword { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage  = "EmailDataType")]
        public string Email { get; set; }
    }
}
