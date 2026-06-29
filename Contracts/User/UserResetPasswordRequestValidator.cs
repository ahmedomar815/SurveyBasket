using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.User;

public class UserResetPasswordRequestValidator : AbstractValidator<UserResetPasswordRequest>
{

    public UserResetPasswordRequestValidator()
    {

        RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x=>x.NewPassword)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 digits and should contains LowerCase, NonAlphanumeric,and uppercase");
    



}

}
