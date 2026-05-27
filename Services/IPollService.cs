namespace SurveyBasket.Services
{
    public interface IPollService
    {
      Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
       Task<Result<Poll>> GetAsync(int Id, CancellationToken cancellationToken = default);
        Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken=default);
       Task<Result> UpdateAsync(int Id, PollRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default);
        Task<Result>TogglePublishStatusAsync(int Id, CancellationToken cancellationToken = default);
    }
}
