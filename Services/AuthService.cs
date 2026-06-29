
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using SurveyBasket.Authentication;
using SurveyBasket.Contracts.User;
using SurveyBasket.Entities;
using SurveyBasket.Errors;
using SurveyBasket.Helpers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services
{
    public class AuthService(UserManager<ApplicationUser> user
        ,SignInManager<ApplicationUser> signInManager,IJwtProvider jwtProvider
        ,ILogger<AuthService> logger ,IEmailSender emailSender
        ,IHttpContextAccessor httpContextAccessor
        ,ApplicationDbContext contxt) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = user;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private IJwtProvider _JwtProvider = jwtProvider;
        private readonly int _refreshTokenExpiryDays = 14;

        private readonly ILogger<AuthService> _logger  = logger;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor= httpContextAccessor;
        private readonly ApplicationDbContext _contxt = contxt;

        public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken)
        {
            var currentUser = await _userManager.FindByEmailAsync(Email);
            if (currentUser == null) 
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            if(currentUser.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);
            var result = await _signInManager.PasswordSignInAsync(currentUser, Password, false, false);
            if (result.Succeeded)

            {
               var (userRoles, userPermissions) = await GetUserRolesAndPermissions(currentUser,cancellationToken);

            var TokenResult = _JwtProvider.GenerateToken(currentUser,userRoles,userPermissions);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
                currentUser.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpiresOn = refreshTokenExiration });
                await _userManager.UpdateAsync(currentUser);
                var response = new AuthResponse(currentUser.Id, currentUser.Email, currentUser.FirstName, currentUser.LastName,
                     TokenResult.Token, TokenResult.ExpiresIn, refreshToken, refreshTokenExiration);
                return Result.Success(response);
            }
            var error = result.IsNotAllowed ?
                UserErrors.EmailNotConfirmed : result.IsLockedOut ?
                UserErrors.LockedUser : UserErrors.InvalidCredentials;
            return Result.Failure<AuthResponse>(error);

        }
        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken)
        {
            var userId = _JwtProvider.ValidateToken(token);
            if (userId is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);
            if(user.LockoutEnd>DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedUser);
            var userrefreshToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshtoken && x.IsActive);
            if (userrefreshToken is null) return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            userrefreshToken.RovkedOn = DateTime.UtcNow;
            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (newToken, expiresIn) = _JwtProvider.GenerateToken(user, userRoles, userPermissions);
            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
            user.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, ExpiresOn = refreshTokenExiration });
            await _userManager.UpdateAsync(user);
            var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName,
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

        public async Task<Result> RegisterAsync(SurveyBasket.Contracts.Authentication.RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var emailIsExists= await _userManager.FindByEmailAsync(request.Email);
            if (emailIsExists != null) return Result.Failure<AuthResponse>(UserErrors.DuplicatedEmail);
            var user=request.Adapt<ApplicationUser>();

            var result= await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                await SendConfirmationEmail(user, code);


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
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user,DefaultRoles.Member);
                return Result.Success();
            }
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ResendConfirmationEmailAsync(ResentConifrmationEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return Result.Success();
            if(user.EmailConfirmed) return Result.Failure(UserErrors.DulicatedConifrmedEmail);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            await SendConfirmationEmail(user, code);
            return Result.Success();
        }
        public async Task<Result> SendResetPasswordCodeAsync(string Email)
        {
            if( await _userManager.FindByEmailAsync(Email) is not { } user) return Result.Success();
            if(!user.EmailConfirmed) return Result.Failure(UserErrors.EmailNotConfirmed);
            var code=await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            _logger.LogInformation("Reset password code for user {Email} is {Code}", Email, code);
            await SendResetPasswordEmail(user, code);
            return Result.Success();
        }
        private async Task<(IEnumerable<string>roles,IEnumerable<string>permission)> GetUserRolesAndPermissions(ApplicationUser user,CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var userPermissions = await _contxt.Roles.Join(_contxt.RoleClaims, r => r.Id, rc => rc.RoleId, (Role, Claim) => new { Role, Claim })
                 .Where(x => userRoles.Contains(x.Role.Name!))
                 .Select(x => x.Claim.ClaimValue!)
                 .Distinct()
                 .ToListAsync(cancellationToken);
            return (userRoles, userPermissions);
        }
        private async Task SendConfirmationEmail(ApplicationUser user, string code)
        {
           
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                    {"{{action_url}}",$"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }

                });
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket:Email Confirmation", emailBody));
           
        }
        private async Task SendResetPasswordEmail(ApplicationUser user, string code)
        {

            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", new Dictionary<string, string>
                {
                    {"{{name}}",user.FirstName },
                    {"{{action_url}}",$"{origin}/auth/forgetPassword?userEmail={user.Email}&code={code}" }

                });
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket:ChangePassword", emailBody));

        }
        public async Task<Result> ResetPasswordAsync(UserResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);
            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }
            if (result.Succeeded)
                return Result.Success();
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));

        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

       
    }
}
