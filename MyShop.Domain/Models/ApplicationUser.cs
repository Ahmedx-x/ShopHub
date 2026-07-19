using Microsoft.AspNetCore.Identity;

namespace MyShop.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}
