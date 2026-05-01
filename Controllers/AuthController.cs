using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Services;

namespace SurveyBasket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController (IAuthService authService): ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        public async Task<IActionResult>LoginAsync(LoginRequest loginRequest,CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetTokenAsync(loginRequest.Email,loginRequest.Password, cancellationToken);
            return authResult == null ? BadRequest("invalid Email or Password") : Ok(authResult);
        }
    }
}
