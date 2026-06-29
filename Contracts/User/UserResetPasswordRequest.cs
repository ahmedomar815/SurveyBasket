namespace SurveyBasket.Contracts.User;

public record UserResetPasswordRequest(string Email, string Code, string NewPassword);
