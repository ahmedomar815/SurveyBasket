namespace SurveyBasket.Contracts.Results
{
    public record VoteResponse(string VoterName,DateTime datetime,IEnumerable<QuestionAnswerResponse> QuestionAnswerResponses);
}
