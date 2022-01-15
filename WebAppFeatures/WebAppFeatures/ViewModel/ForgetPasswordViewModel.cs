using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.ViewModel
{
    public class ForgetPasswordViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "EmailDataType")]
        public string Email { get; set; }
    }
}
