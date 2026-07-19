using Microsoft.AspNetCore.Identity;
using MyShop.Business.DTOs;

namespace MyShop.Business.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<SignInResult> LoginAsync(LoginDto dto);
        Task LogoutAsync();
    }
}
