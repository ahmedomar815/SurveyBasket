
using Mapster;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Errors;
using SurveyBasket.Persistence;

namespace SurveyBasket.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {

        private readonly ApplicationDbContext _context = context;


        public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Polls!.AsNoTracking().ToListAsync();

        public async Task<Result<Poll>> GetAsync(int Id, CancellationToken cancellationToken)
        {
            var poll=await _context.Polls!.FindAsync(Id, cancellationToken);
            return poll is not null ? Result.Success(poll) : Result.Failure<Poll>(PollErrors.PollNotFound);

        }
        public async Task<Result<Poll>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = poll.Adapt<Poll>();

                await _context.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(entity);
            }
            catch
            {
                return Result.Failure<Poll>(PollErrors.CreationFailed);
            }
        }
        public async Task<Result> UpdateAsync(int Id, PollRequest poll, CancellationToken cancellationToken = default)
        {
           var currentPoll=await _context.Polls!.FindAsync(Id, cancellationToken);
            if (currentPoll is null)
                return Result.Failure(PollErrors.PollNotFound);
              currentPoll.Title = poll.Title;
            currentPoll.Summary = poll.Summary;
            currentPoll.StartsAt = poll.StartsAt;
            currentPoll.EndsAt = poll.EndsAt;
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default)
        {
           var result=await GetAsync(Id,cancellationToken);
            if(result.IsFailure) return Result.Failure(result.Error);
            var poll = result.Value;
                _context.Remove(poll);
                await _context.SaveChangesAsync();
                return Result.Success();
        }
        public async Task<Result> TogglePublishStatusAsync(int Id, CancellationToken cancellationToken = default)
        {
            var result = await GetAsync(Id, cancellationToken);
            if (result.IsFailure) return Result.Failure(result.Error);
             var currentPoll = result.Value;
            currentPoll.IsPublished = !currentPoll.IsPublished;
             await _context.SaveChangesAsync();  
             return Result.Success();
        }

    }
}
