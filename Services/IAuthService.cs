
namespace SurveyBasket.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token,string refreshtoken, CancellationToken cancellationToken);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken);
    }
}
