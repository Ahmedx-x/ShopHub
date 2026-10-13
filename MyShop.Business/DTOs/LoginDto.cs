using System.ComponentModel.DataAnnotations;

namespace MyShop.Business.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }
}
