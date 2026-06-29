namespace SurveyBasket.Contracts.User;

public class UserForgetPasswordRequestValidator : AbstractValidator<UserForgetPasswordRequest>
{

    public UserForgetPasswordRequestValidator()
    {


        RuleFor(x => x.email)
            .NotEmpty()
            .EmailAddress();

       
    }

}
