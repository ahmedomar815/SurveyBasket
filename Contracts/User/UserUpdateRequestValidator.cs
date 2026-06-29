using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Contracts.User;

public class UserUpdateRequestValidator   : AbstractValidator<UserUpdateRequest>
{

    public UserUpdateRequestValidator()
    {


        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);
     
        RuleFor(x=>x.LastName)  
            .NotEmpty()
            .Length(3,100);

}

}

