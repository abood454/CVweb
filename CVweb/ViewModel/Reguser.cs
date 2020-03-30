using CVweb.Models;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CVweb.ViewModel
{
    public class Reguser
    {
        public int id { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        public string name { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        [DataType(DataType.Password)]
        public string password { get; set; }

        [Compare("password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string copassword { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        public string phone { get; set; }
        
        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogin { get; set; }
    }
}
