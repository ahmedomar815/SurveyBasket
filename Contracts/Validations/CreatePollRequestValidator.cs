using FluentValidation;

namespace SurveyBasket.Contracts.Validations
{
    public class CreatePollRequestValidator:AbstractValidator<createPollRequest>
    {
        public CreatePollRequestValidator() 
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("please add a title {PropertyName}")
                .Length(3, 100).WithMessage("Title should be at least {MinLength} and  mixmum {MaxLength}, you entered[{PropertyValue}]");

            RuleFor(c => c.Description)
             .NotEmpty().Length(3, 1000);
        }
    }
}
 