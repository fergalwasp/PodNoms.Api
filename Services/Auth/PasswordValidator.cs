using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using PodNoms.Api.Models;
using static IdentityModel.OidcConstants;

namespace PodNoms.Api.Services.Auth
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, context.Password))
            {
                context.Result = new GrantValidationResult(user.Id.ToString(), "password");
                return;
            }

            context.Result = new GrantValidationResult(TokenErrors.InvalidRequest, "Username or password is incorrect");
        }
    }
}