
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SurveyBasket.Authentication;
using SurveyBasket.Entities;
using SurveyBasket.Errors;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services
{
    public class AuthService(UserManager<ApplicationUser> user,SignInManager<ApplicationUser> signInManager,IJwtProvider jwtProvider,ILogger<AuthService> logger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = user;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private IJwtProvider _JwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;

        private readonly ILogger<AuthService> _logger  = logger;

        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByEmailAsync(Email);
            if (currentUser == null) 
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var result = await _signInManager.PasswordSignInAsync(currentUser, Password, false, false);
            if (result.Succeeded)

            {

                var TokenResult = _JwtProvider.GenerateToken(currentUser);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                currentUser.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpiresOn = refreshTokenExiration });
                await _userManager.UpdateAsync(currentUser);
                var response = new AuthResponse(currentUser.Id, currentUser.Email, currentUser.FirstName, currentUser.LastName,
                     TokenResult.Token, TokenResult.ExpiresIn, refreshToken, refreshTokenExiration);
                return Result.Success(response);
            }
            return result.IsNotAllowed? Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed):
                Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        }
        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
        {
            var userId = _JwtProvider.ValidateToken(token);
            if (userId is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var userrefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshtoken && x.IsActive);
            if (userrefreshToken is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            userrefreshToken.RovkedOn = DateTime.UtcNow;

            var (newToken, expiresIn) = _JwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiresOn = refreshTokenExiration });
            await _userManager.UpdateAsync(user);
            var authResponse= new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName,
                newToken, expiresIn, newRefreshToken, refreshTokenExiration);
            return Result.Success<AuthResponse>(authResponse);

        }
        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
        {

            var userId = _JwtProvider.ValidateToken(token);
            if (userId is null) return Result.Failure(UserErrors.InvalidCredentials);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Failure(UserErrors.InvalidCredentials);
            var userrefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshtoken && x.IsActive);
            if (userrefreshToken is null) return Result.Failure(UserErrors.InvalidCredentials);
            userrefreshToken.RovkedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return Result.Success();

        }

        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken=default)
        {
            var emailIsExists= await _userManager.FindByEmailAsync(request.Email);
            if (emailIsExists != null) return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);
            var user=request.Adapt<ApplicationUser>();

            var result= await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var code= await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description,StatusCodes.Status400BadRequest));


        }
        public async Task<Result>ConfirmEmailAsync(ConfirmEmailRequest confirmEmailReuqest)
        {
            var user = await _userManager.FindByIdAsync(confirmEmailReuqest.Id);
            if (user== null) return Result.Failure(UserErrors.InvalidCode);
            if (user.EmailConfirmed) return Result.Failure(UserErrors.DulicatedConifrmedEmail);
            var code = confirmEmailReuqest.code;
            try
            {
                code=Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            }
            catch(FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded) return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ResendConfirmationEmailAsync(ResentConifrmationEmailRequest request)
        {
            var userIsExists = await _userManager.FindByEmailAsync(request.Email);
            if (userIsExists is null) return Result.Success();
            if(userIsExists.EmailConfirmed) return Result.Failure(UserErrors.DulicatedConifrmedEmail);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userIsExists);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return Result.Success();
        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

       
    }
}
