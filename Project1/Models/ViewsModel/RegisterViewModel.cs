using System.ComponentModel.DataAnnotations;
namespace Project1.Models.ViewsModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Вкажіть ім'я")]
        [MaxLength(20, ErrorMessage = "Ім'я повинно бути менше 20 літер")]
        [MinLength(3, ErrorMessage = "Ім'я повинно бути не менше 3 літер")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Вкажіть пошту")]
        [MaxLength(30, ErrorMessage = "Пошта повинна бути менше 30 літер")]
        [MinLength(8, ErrorMessage = "Пошта повинна бути не менше 8 літер")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Вкажіть пароль")]
        [MinLength(6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пітвердіть пароль")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string PasswordConfirm { get; set; }
    }
}
