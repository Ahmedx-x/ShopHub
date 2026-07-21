using MyShop.Business.DTOs;

namespace MyShop.Business.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> DisplayAllUsers();
        Task CustomerToAdmin(string userId);
        Task AdminToCustomer(string userId);
        Task LockUserAccount(string userId);
        Task UnLockUserAccount(string userId);
        Task DeleteAccount(string userId);
    }
}
