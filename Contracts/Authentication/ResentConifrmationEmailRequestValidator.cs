
namespace SurveyBasket.Contracts.Authentication
{
    public class ResentConifrmationEmailRequestValidator:AbstractValidator<ResentConifrmationEmailRequest>
    {

        public ResentConifrmationEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();


        }
    
    }
}
