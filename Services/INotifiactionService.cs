

namespace SurveyBasket.Services;

public interface INotifiactionService
{
    Task SendNewPollsNoification(int? pollId);
}
