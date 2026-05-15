namespace SurveyBasket.Services
{
    public interface IPollService
    {
      Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
       Task<Result<Poll>> GetAsync(int Id, CancellationToken cancellationToken = default);
        Task<Result<Poll>> AddAsync(PollRequest poll, CancellationToken cancellationToken=default);
       Task<Result> UpdateAsync(int Id, PollRequest poll, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default);
        Task<Result>TogglePublishStatusAsync(int Id, CancellationToken cancellationToken = default);
    }
}
