namespace SurveyBasket.Contracts.User;

public record UserChangePasswordRequest( string currentPassword, string newPassword);
