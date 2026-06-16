namespace SurveyBasket.Contracts.Results
{
    public record VotePerQuestionResponse(string Question,IEnumerable<VotesPerAnswerResponse> SelectedAnswers);
}
