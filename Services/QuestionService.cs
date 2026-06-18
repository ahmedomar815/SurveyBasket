



using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;
using System.Collections.Generic;
using System.Linq;

namespace SurveyBasket.Services
{
    public class QuestionService(ApplicationDbContext context,  HybridCache hybrideCache) : IQuestionService
    {
        private readonly ApplicationDbContext _context = context;
     
        private const string _cachePrefix = "availableQuestion";

        public HybridCache _hybrideCache = hybrideCache;

        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
        {
             var pollIsExists =  await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
             if(!pollIsExists) return  Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
            var question = await _context.Questions
          .Where(q => q.PollId == pollId && q.Id == questionId)
          .Include(q => q.Answers)
           .ProjectToType<QuestionResponse>()
           .AsNoTracking()
           .SingleOrDefaultAsync(cancellationToken); 
            return question is null ? Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound) : Result.Success(question);
        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvliableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
        {
           /* var hasVote = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);
            if (hasVote) return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

            var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId &&
            p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
            if (!pollIsExists)*/
                /*return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);*/
            var cacheKey = $"{_cachePrefix}-{pollId}";


            var question = await _hybrideCache.GetOrCreateAsync< IEnumerable < QuestionResponse >> (cacheKey, async cacheEntry =>
            {
               

                return  await _context.Questions
                 .Where(q => q.PollId == pollId && q.IsActive)
                 .Include(x => x.Answers)
                 .Select(q => new QuestionResponse
                 (
                     q.Id,
                      q.Content,
                      q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse
                     (
                         a.Id,
                         a.Content
                     )).ToList()
                 )).AsNoTracking()
                 .ToListAsync(cancellationToken);
            }
             
            );



            return Result.Success<IEnumerable<QuestionResponse>>(question!);
        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);
            if (!pollIsExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);
            var questions = await _context.Questions
           .Where(x => x.PollId == pollId)
           .Include(x => x.Answers)
           .AsNoTracking()
           .ToListAsync(cancellationToken);
            
            return Result.Success<IEnumerable<QuestionResponse>>(questions.Adapt<IEnumerable<QuestionResponse>>());
        }
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
            if (!pollIsExists)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);


            var questionIsExists = await _context.Questions.AnyAsync(q => q.Content == request.Content&&q.PollId == pollId, cancellationToken);
            if (questionIsExists)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestonContent);

            var question = request.Adapt<Question>();
            question.PollId = pollId;
            await _context.Questions.AddAsync(question, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
           await _hybrideCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
            return Result.Success(question.Adapt<QuestionResponse>());

        }
        public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
              var questionIsExists= await _context.Questions.AnyAsync
                (q=>q.PollId == pollId &&q.Id!= questionId&&q.Content== request.Content, cancellationToken);
               if (questionIsExists)
                return Result.Failure(QuestionErrors.DuplicatedQuestonContent);
                
               var question= await _context.Questions.Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, cancellationToken);
                if (question is null) 
                    return Result.Failure(QuestionErrors.QuestionNotFound);

                question.Content = request.Content;

                var CurrentAnswers = question.Answers.Select(x=> x.Content).ToList();   
                var newAnswers = request.Answers.Except(CurrentAnswers).ToList();
                newAnswers.ForEach(a => question.Answers.Add(new Answer { Content = a }));
                question.Answers.ToList().ForEach(a =>
                {
                    a.IsActive = request.Answers.Contains(a.Content);
                });
            await _hybrideCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

            return Result.Success();
        }
        public async Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default)
        {
             var question= await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, cancellationToken);
             if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
             question.IsActive = !question.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            await _hybrideCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);
            return Result.Success();
        }

        
    }
}
