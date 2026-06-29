using SurveyBasket.Contracts.User;

namespace SurveyBasket.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UserUpdateRequest request);
    Task<Result> ChangePasswordAsync(string userId, UserChangePasswordRequest request);
    Task<Result<UserResponse>> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> AddAsync(CreateUsreRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string Id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatus(string Id,CancellationToken cancellationToken = default);
    Task<Result> UnlockUser(string Id, CancellationToken cancellationToken = default);
}
