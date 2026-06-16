

namespace SurveyBasket.Services
{
    public interface IResultService
    {
        Task<Result<PollVoteResponse>> GetPollVotesResultAsync(int pollId, CancellationToken cancallationToken);
        Task<Result<IEnumerable<VotesPerDayResponse>>>GetVotesPerDayAsync(int pollId, CancellationToken cancallationToken);
        Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotePerQuestionAsync(int pollId, CancellationToken cancallationToken);
    }
}
