using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RWAT.Models
{
    public class RegisterModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(10,MinimumLength=6, ErrorMessage ="Password should be at least, six characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "Password should be at least, six characters long")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}