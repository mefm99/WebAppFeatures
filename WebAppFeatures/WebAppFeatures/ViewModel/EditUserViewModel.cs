using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppFeatures.ViewModel
{
    public class EditUserViewModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Full Name is Required ")]
        [Display(Name = "Full Name")]
        [MinLength(5, ErrorMessage = "Min Length 5 Char")]
        [MaxLength(25, ErrorMessage = "Max Length 25 Char")]
        public string FullName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is Required ")]
        [Display(Name = "User Name")]
        [MinLength(5, ErrorMessage = "Min Length 5 Char")]
        [MaxLength(25, ErrorMessage = "Max Length 25 Char")]
        [RegularExpression("^[A-Za-z]\\w{5,25}$", ErrorMessage = "Username is not supported ")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Key { get; set; }
    }
}
