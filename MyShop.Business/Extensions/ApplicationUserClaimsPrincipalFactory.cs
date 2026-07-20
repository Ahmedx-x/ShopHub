using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyShop.Domain.Models;
using System.Security.Claims;

namespace MyShop.Business.Extensions
{
    public class ApplicationUserClaimsPrincipalFactory :
        UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {

        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            // default claims (Id, UserName, Role and etc..)
            var principal = await base.CreateAsync(user);

            // add FullName property to user claims
            ((ClaimsIdentity)principal.Identity!).AddClaim(
                new Claim("FullName", user.FullName ?? string.Empty));

            return principal;
        }
    }
}
