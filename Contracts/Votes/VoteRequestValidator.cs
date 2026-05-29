namespace SurveyBasket.Contracts.Votes
{
    public class VoteRequestValidator:AbstractValidator<VoteRequest>
    {
        public VoteRequestValidator()
        {
            RuleFor(x => x.answers)
                .NotEmpty();
            RuleForEach(x => x.answers)
                .SetValidator((new VoteAnswerRequestValidator()));

        }
    }
}
