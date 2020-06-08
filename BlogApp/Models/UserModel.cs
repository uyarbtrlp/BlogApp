using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApp.Models
{
    public class UserModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "İsim gereklidir.")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Soyad gereklidir.")]

        public string Surname { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Lütfen email adresinizi düzgün giriniz")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Şifre gereklidir.")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Telefon numarası gereklidir.")]
        public string PhoneNumber { get; set; }
        public string Date { get; set; }
    }
}
