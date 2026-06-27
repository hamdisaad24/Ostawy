using System.ComponentModel.DataAnnotations;

namespace Ostawy.ViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "الايميل مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة الايميل غير صحيحة")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }    

    }
}
