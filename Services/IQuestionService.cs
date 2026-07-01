using SurveyBasket.Contracts.Commons;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services
{
    public interface IQuestionService
    {

        
        Task<Result<QuestionResponse>>GetAsync(int pollId,int questionId, CancellationToken cancellationToken=default);
        Task<Result<IEnumerable<QuestionResponse>>> GetAvliableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<QuestionResponse>>>GetAllAsync(int pollId, RequestFilter request,CancellationToken cancellationToken=default);
        Task<Result<QuestionResponse>>AddAsync(int pollId,QuestionRequest request, CancellationToken cancellationToken=default);
        Task<Result>UpdateAsync(int pollId,int questionId, QuestionRequest request, CancellationToken cancellationToken=default);
        Task<Result>ToggleStatusAsync(int pollId,int questionId, CancellationToken cancellationToken=default);
        
    }
}
