namespace SurveyBasket.Contracts.User;

public record CreateUsreRequest(string FirstName, string LastName, string Email, string Password, List<string> Roles);
