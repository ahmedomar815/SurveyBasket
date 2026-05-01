using SurveyBasket.Entities;

namespace SurveyBasket.Authentication
{
    public interface IJwtProvider
    {
        (string Token, int ExpiresIn) GenerateToken(ApplicationUser user);
        
    }
}
