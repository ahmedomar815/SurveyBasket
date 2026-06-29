using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Contracts.User;

namespace SurveyBasket.Controllers;

[Route("me")]
[ApiController]
[Authorize]
public class AccountController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result= await _userService.GetProfileAsync(User.GetUserId()!);

        return Ok(result.Value);
    }
    [HttpPut("update-info")]
    public async Task<IActionResult> UpdateProfile([FromBody]UserUpdateRequest request)
    {
       await _userService.UpdateProfileAsync(User.GetUserId()!, request);

        return NoContent();
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
        return result.IsSuccess? NoContent() : result.ToProblem();
    }
}

