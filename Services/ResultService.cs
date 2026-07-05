

namespace SurveyBasket.Services
{
    public class ResultService (ApplicationDbContext context ) : IResultService
    {
        public ApplicationDbContext _context = context;

        public async Task<Result<PollVoteResponse>> GetPollVotesResultAsync(int pollId, CancellationToken cancallationToken)
        {
            var pollvote = await _context.Polls.Where(p => p.Id == pollId)
               .Select(p => new PollVoteResponse
               (
                 p.Title,
                 p.Votes.Select(v => new VoteResponse
                 (
                    v.User.FirstName + " " + v.User.LastName,
                    v.SumittedOn,
                    v.VoteAnswers.Select(va => new QuestionAnswerResponse
                    (
                       va.Question.Content,
                       va.Answer.Content
                       ))
                 ))
               )).SingleOrDefaultAsync(cancallationToken);
            return pollvote is null ? Result.Failure<PollVoteResponse>(PollErrors.PollNotFound) : Result.Success(pollvote);
        }

        public async Task<Result<IEnumerable<VotePerQuestionResponse>>> GetVotePerQuestionAsync(int pollId, CancellationToken cancallationToken)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancallationToken);

            if (!pollIsExists) return Result.Failure<IEnumerable<VotePerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = await _context.VoteAnswers.Where(va => va.Vote.PollId == pollId)
                .Select(vs => new VotePerQuestionResponse
                (
                       vs.Question.Content,
                       vs.Question.VoteAnswers.GroupBy(v => new {AnswerId= v.AnswerId,AnswerContent=v.Answer.Content})
                       .Select(g => new VotesPerAnswerResponse
                       (
                          g.Key.AnswerContent,
                          g.Count()
                       ))
                )).ToListAsync(cancallationToken);

            return Result.Success<IEnumerable<VotePerQuestionResponse>>(votesPerQuestion);

        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync([FromRoute]int pollId, CancellationToken cancallationToken)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancallationToken);

            if (!pollIsExists) return Result.Failure<IEnumerable <VotesPerDayResponse>>(PollErrors.PollNotFound);

            var votesPerDay = await _context.Votes.Where(v => v.PollId == pollId)
                .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SumittedOn) })
                .Select(g => new VotesPerDayResponse
                (
                     g.Key.Date,
                     g.Count()
                )).ToListAsync(cancallationToken);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }


    }
}
