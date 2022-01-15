using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Old Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Old Password Required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Display(Name = "New Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,16}$", ErrorMessage = "Password must contain uppercase and lowercase letters, symbols, numbers, and between 6-18")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name = "New Password Again")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "New Password Again Requied")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage ="Not Match")]
        public string NewPasswordAgain { get; set; }
    }
}
