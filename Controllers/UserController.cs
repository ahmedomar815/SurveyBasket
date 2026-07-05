using Asp.Versioning;
using SurveyBasket.Contracts.User;

namespace SurveyBasket.Controllers;

[ApiVersion(1, Deprecated = true)]
[ApiVersion("2.0")]
[Route("[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(cancellationToken));
    }
    [HttpGet("{userId}")]
    [HasPermission(Permissions.GetUsers)]
    public async Task<IActionResult> Get([FromRoute]string userId,CancellationToken cancellationToken)
    {
        var result=await _userService.GetUserAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();  
    }
    [HttpPost("Add")]
    [HasPermission(Permissions.AddUsers)]
    public async Task<IActionResult> Add([FromBody]CreateUsreRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(request, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(Get),new { userId =result.Value.Id},result.Value): result.ToProblem();
    }
    [HttpPut("{userId}/update")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(userId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpPut("{userId}/toggle-status")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> ToggleStatus([FromRoute]string userId, CancellationToken cancellationToken)
    {
        var result = await _userService.ToggleStatus(userId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpPut("{userId}/unlock")]
    [HasPermission(Permissions.UpdateUsers)]
    public async Task<IActionResult> UnLock([FromRoute] string userId, CancellationToken cancellationToken)
    {
        var result = await _userService.UnlockUser(userId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}

