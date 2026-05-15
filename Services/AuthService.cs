
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SurveyBasket.Authentication;
using SurveyBasket.Entities;
using SurveyBasket.Errors;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SurveyBasket.Services
{
    public class AuthService(UserManager<ApplicationUser> user,IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _user = user;

        private IJwtProvider _JwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;
        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken)
        {
            var currentUser = await _user.FindByEmailAsync(Email);
            if (currentUser == null) 
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var checkPassword = await _user.CheckPasswordAsync(currentUser, Password);
            if (!checkPassword)
               return Result.Failure<AuthResponse>(new Error("User.InvalidCredentials", "Invalid email/password"));

            var TokenResult = _JwtProvider.GenerateToken(currentUser);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            currentUser.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpiresOn = refreshTokenExiration });
            await _user.UpdateAsync(currentUser);
           var response=new AuthResponse(currentUser.Id, currentUser.Email, currentUser.FirstName, currentUser.LastName,
                TokenResult.Token, TokenResult.ExpiresIn, refreshToken, refreshTokenExiration);
            return Result.Success(response);

        }
        public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
        {
            var userId = _JwtProvider.ValidateToken(token);
            if (userId is null) return null;
            var user = await _user.FindByIdAsync(userId);
            if (user is null) return null;
            var userrefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshtoken && x.IsActive);
            if (userrefreshToken is null) return null;
            userrefreshToken.RovkedOn = DateTime.UtcNow;

            var (newToken, expiresIn) = _JwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiresOn = refreshTokenExiration });
            await _user.UpdateAsync(user);
            return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName,
                newToken, expiresIn, newRefreshToken, refreshTokenExiration);

        }
        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
        {

            var userId = _JwtProvider.ValidateToken(token);
            if (userId is null) return false;
            var user = await _user.FindByIdAsync(userId);
            if (user is null) return false;
            var userrefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshtoken && x.IsActive);
            if (userrefreshToken is null) return false;
            await _user.UpdateAsync(user);
            return true;

        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

       
    }
}
