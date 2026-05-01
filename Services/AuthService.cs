
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using SurveyBasket.Entities;

namespace SurveyBasket.Services
{
    public class AuthService(UserManager<ApplicationUser> user,IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _user = user;

        public IJwtProvider _JwtProvider = jwtProvider;

        public async Task<AuthResponse?> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken)
        {
           var currentUser= await _user.FindByEmailAsync(Email);
            if (currentUser == null) return null;
             var checkPassword =  await _user.CheckPasswordAsync(currentUser, Password);
            if (!checkPassword) return null;

            var TokenResult = _JwtProvider.GenerateToken(currentUser);
            return new AuthResponse(currentUser.Id, currentUser.Email, currentUser.FirstName, currentUser.LastName, TokenResult.Token, TokenResult.ExpiresIn);

        }
    }
}
