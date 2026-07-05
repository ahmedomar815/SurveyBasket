
namespace SurveyBasket.Authentication
{
    public interface IJwtProvider
    {
        (string Token, int ExpiresIn) GenerateToken(ApplicationUser user,IEnumerable<string>roles, IEnumerable<string> permissions);
        string? ValidateToken(string Token);
        
    }
}
