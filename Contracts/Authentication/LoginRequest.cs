namespace SurveyBasket.Contracts.Authentication
{
    public record LoginRequest(
        string Eamil,
        string Password);
}
