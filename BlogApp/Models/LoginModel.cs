using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Username { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Şifre gereklidir.")]
        public string Password
        {
            get; set;
        }
    }
}
