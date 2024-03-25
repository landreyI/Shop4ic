using System.ComponentModel.DataAnnotations;

namespace Project1.Models.ViewsModel
{
    public class AuthorizationViewModel
    {
        
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Вкажіть пароль")]
        [MinLength(6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
        public string Password { get; set; }
    }
}
