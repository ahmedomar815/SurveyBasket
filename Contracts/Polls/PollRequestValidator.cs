using FluentValidation;

namespace SurveyBasket.Contracts.Polls
{
    public class LoginRequestValidator:AbstractValidator<PollRequest>
    {
        
        public LoginRequestValidator() 
        {
            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("please add a title {PropertyName}")
                .Length(3, 100).WithMessage("Title should be at least {MinLength} and  mixmum {MaxLength}, you entered[{PropertyValue}]");

            RuleFor(c => c.Summary)
             .NotEmpty().Length(3, 1000);
            RuleFor(x => x.StartsAt).NotEmpty().GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

            RuleFor(x => x).Must(HasValidDates).WithName(nameof(PollRequest.EndsAt))
                .WithMessage("{PropertyName} must be greater than or equals start date");

        }
        private bool HasValidDates(PollRequest pollRequest)
        {
            return pollRequest.StartsAt <= pollRequest.EndsAt;
        }
    }
}
 