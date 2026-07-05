

using Hangfire;


namespace SurveyBasket.Services
{
    public class PollService(ApplicationDbContext context,INotifiactionService notifiactionService) : IPollService
    {

        private readonly ApplicationDbContext _context = context;
        private readonly INotifiactionService _notifiactionService = notifiactionService;

        public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _context.Polls
                .AsNoTracking()
                .ProjectToType<PollResponse>()
                .ToListAsync(cancellationToken);
        public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)
            => await GetQuery()
                .ProjectToType<PollResponse>()
                .ToListAsync(cancellationToken);
        public async Task<IEnumerable<PollResponseV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
           => await GetQuery()
               .ProjectToType<PollResponseV2>()
               .ToListAsync(cancellationToken);

        public async Task<Result<Poll>> GetAsync(int Id, CancellationToken cancellationToken)
        {
            var poll=await _context.Polls!.FindAsync(Id, cancellationToken);
            return poll is not null ? Result.Success(poll) : Result.Failure<Poll>(PollErrors.PollNotFound);

        }
        public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
        {
            var isExistingTitle=await _context.Polls!.AnyAsync(p => p.Title == request.Title, cancellationToken);
            if (isExistingTitle)
                return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
            var poll = request.Adapt<Poll>();
            await _context.AddAsync(poll, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success (poll.Adapt<PollResponse>());
          
        }
        public async Task<Result> UpdateAsync(int Id, PollRequest request, CancellationToken cancellationToken = default)
        {
            var isExistingTitle = await _context.Polls!.AnyAsync(p => p.Title == request.Title && p.Id!= Id, cancellationToken);
            if (isExistingTitle)
                return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);
            var currentPoll=await _context.Polls!.FindAsync(Id, cancellationToken);
            if (currentPoll is null)
                return Result.Failure(PollErrors.PollNotFound);
            currentPoll = request.Adapt(currentPoll);
            await _context.SaveChangesAsync(cancellationToken);
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
            if (currentPoll.IsPublished && currentPoll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                BackgroundJob.Enqueue(() => _notifiactionService.SendNewPollsNoification(currentPoll.Id));
             return Result.Success();
        }

        private IQueryable<Poll> GetQuery()
        {
            var query = _context.Polls
                .AsNoTracking()
                .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));
            return query;
        }
    }
}
