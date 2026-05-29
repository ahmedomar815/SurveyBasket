using Microsoft.EntityFrameworkCore;
using SurveyBasket.Contracts.Questions;
using SurveyBasket.Contracts.Votes;

namespace SurveyBasket.Services
{
    public class VoteService(ApplicationDbContext context) : IVoteService
    {
        public ApplicationDbContext _context = context;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken)
        {
            var hasVote = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
            if (hasVote) return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

            var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId &&
            p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
            if (!pollIsExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var availableQuestions = await _context.Questions.Where(x => x.PollId == pollId && x.IsActive)
                .Select(x => x.Id)
                .ToListAsync();
            if (request.answers.Select(x => x.QuestionId).SequenceEqual(availableQuestions))
                return Result.Failure(VoteErrors.InvaidQuestions);

            Vote vote = new Vote
            {
                UserId = userId,
                PollId = pollId,
                VoteAnswers = request.answers.Adapt<IEnumerable<VoteAnswer>>().ToList()

            };
            await _context.Votes.AddAsync(vote, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
