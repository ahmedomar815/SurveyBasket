using Asp.Versioning;
using SurveyBasket.Contracts.Roles;

namespace SurveyBasket.Controllers;

[ApiVersion(1, Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet("")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisable, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllRolesAsync(includeDisable, cancellationToken);
        return Ok(roles);
    }
    [HttpGet("{roleId}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetRole([FromRoute] string roleId)
    {
        var result = await _roleService.GetRole(roleId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("add-role")]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> Add(RoleRequest request)
    {
        var result = await _roleService.AddRoleAsync(request);
        return result.IsSuccess ? CreatedAtAction(nameof(GetRole), new { result.Value.Id }, result.Value) : result.ToProblem();
    }
    [HttpPut("{rollId}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update(string rollId,RoleRequest request)
    {
        var result = await _roleService.UpdateRoleAsync(rollId, request);
        return result.IsSuccess ? NoContent(): result.ToProblem();
    }
    [HttpPut("{rollId}/toggle-status")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus(string rollId)
    {
        var result = await _roleService.ToggleStatusAsync(rollId);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
