
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Entities;

namespace SurveyBasket.Services
{
    public class AuthService(UserManager<ApplicationUser>userManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;

        public async Task<AuthResponse?> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken)
        {
            var user=await userManager.FindByEmailAsync(Email);
            if (user == null) return null;
            var isValidPassword=  await userManager.CheckPasswordAsync(user, Password);
            if(isValidPassword == false) return null;

            return new AuthResponse(user.Id,user.Email,user.FirstName,user.LastName,"token" ,30);
        }
    }
}
