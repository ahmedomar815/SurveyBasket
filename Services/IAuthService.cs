
namespace SurveyBasket.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken);
        Task<AuthResponse?> GetRefreshTokenAsync(string token,string refreshtoken, CancellationToken cancellationToken);
        Task<bool> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken);
    }
}
