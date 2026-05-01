
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Persistence;

namespace SurveyBasket.Services
{
    public class PollService(ApplicationDbContext context) : IPollService
    {

        private readonly ApplicationDbContext _context = context;


        public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Polls!.AsNoTracking().ToListAsync();

        public async Task<Poll?> GetAsync(int Id, CancellationToken cancellationToken) => await _context.Polls!.FindAsync(Id);
        public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(poll);
            await _context.SaveChangesAsync();
            return poll;
        }
        public async Task<bool> UpdateAsync(int Id, Poll poll, CancellationToken cancellationToken = default)
        {
           var currentPoll=await GetAsync(Id, cancellationToken);
           if(currentPoll is null) return false;
              currentPoll.Title = poll.Title;
            currentPoll.Summary = poll.Summary;
            currentPoll.StartsAt = poll.StartsAt;
            currentPoll.EndsAt = poll.EndsAt;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int Id, CancellationToken cancellationToken = default)
        {
           var currentPoll=await GetAsync(Id,cancellationToken);
              if(currentPoll is null) return false;
                _context.Remove(currentPoll);
                await _context.SaveChangesAsync();
                return true;
        }
        public async Task<bool> TogglePublishStatusAsync(int Id, CancellationToken cancellationToken = default)
        {
            var currentPoll = await GetAsync(Id, cancellationToken);
            if(currentPoll is null) return false;
            currentPoll.IsPublished = !currentPoll.IsPublished;
            await _context.SaveChangesAsync();  
            return true;
        }

    }
}
