using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.Business.DTOs;
using MyShop.Business.Interfaces;
using MyShop.DataAccess.Context;
using MyShop.Domain.Interfaces;
using MyShop.Domain.Models;

namespace MyShop.Business.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task AdminToCustomer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.Admin);
                if (!removeResult.Succeeded)
                    throw new Exception(string.Join(", ", removeResult.Errors.Select(e => e.Description)));

                var addResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
                if (!addResult.Succeeded)
                    throw new Exception(string.Join(", ", addResult.Errors.Select(e => e.Description)));

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task CustomerToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, Roles.Customer);
                if (!removeResult.Succeeded)
                    throw new Exception(string.Join(", ", removeResult.Errors.Select(e => e.Description)));

                var addResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
                if (!addResult.Succeeded)
                    throw new Exception(string.Join(", ", addResult.Errors.Select(e => e.Description)));

                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                throw new Exception(string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
        }

        public async Task<IReadOnlyList<UserDto>> DisplayAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            // first methos cause N+1 problem
            //var result = new List<UserDto>();
            //foreach (var user in users)
            //{
            //    var userRole = await _userManager.GetRolesAsync(user);
            //    result.Add(new UserDto
            //    {
            //        Id = user.Id,
            //        FullName = user.FullName,
            //        UserName = user.UserName!,
            //        Role = userRole.First(),
            //        LockStatus = user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow
            //    });
            //}

            // second method make a join between
            // Users and UserRoles and Roles in the database and return the result
            var result = await (
                from u in _dbContext.Users
                join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                join r in _dbContext.Roles on ur.RoleId equals r.Id
                select new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName!,
                    Role = r.Name!,
                    LockStatus = u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow
                }
                ).ToListAsync();

            return result;
        }

        public async Task LockUserAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            if (!user.LockoutEnabled)
                await _userManager.SetLockoutEnabledAsync(user, true);

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }

        public async Task UnLockUserAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User '{userId}' not found.");

            if (!user.LockoutEnabled)
                await _userManager.SetLockoutEnabledAsync(user, true);

            await _userManager.SetLockoutEndDateAsync(user, null);
        }
    }
}
