using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.User;

public class UserChangePasswordRequestValidator : AbstractValidator<UserChangePasswordRequest>
{

    public UserChangePasswordRequestValidator()
    {


        RuleFor(x => x.currentPassword)
            .NotEmpty();

        RuleFor(x => x.newPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 digits and should contains LowerCase, NonAlphanumeric,and uppercase ")
            .NotEqual(x=>x.currentPassword)
            .WithMessage("new password cannot be same as the current password");


    }

}
