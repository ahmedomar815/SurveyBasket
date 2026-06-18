namespace SurveyBasket.Contracts.Authentication
{
    public class ConnfirmEmailReuqestValidator : AbstractValidator<ConfirmEmailRequest>
    {

        public ConnfirmEmailReuqestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.code).NotEmpty();



        }
    }
    
}
