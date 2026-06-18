namespace SurveyBasket.Services
{
    public interface ICacheService
    { 
        Task<T?> GetAsync<T>(string key,CancellationToken cancallationToken=default) where T : class;
        Task SetAsync<T>(string Key, T value, CancellationToken cancallationToken = default) where T: class;
        Task RemoveAsync<T>(string Key,CancellationToken cancallationToken=default)    where T: class;

    }


}
