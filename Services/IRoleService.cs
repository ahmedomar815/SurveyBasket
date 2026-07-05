using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailsResponse>> GetRole(string rollId, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailsResponse>> AddRoleAsync(RoleRequest request);
    Task<Result> UpdateRoleAsync(string rollId, RoleRequest request);
    Task<Result> ToggleStatusAsync(string rollId);
}
