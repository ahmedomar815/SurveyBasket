using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace SurveyBasket.Contracts.User;

public record UpdateUserRequest(string FirstName, string LastName, string Email, List<string> Roles);

