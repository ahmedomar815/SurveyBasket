using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SurveyBasket.Abstractions;
using SurveyBasket.Authentication;
using SurveyBasket.Services;

namespace SurveyBasket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService,ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        public readonly ILogger<AuthController> _logger  = logger;

        [HttpPost("")]
        public async Task<IActionResult> LoginaAsync([FromBody]LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for email: {Email} and password :{password} ", loginRequest.Email,loginRequest.Password);
            var authResult = await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
            return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return authResult.IsSuccess?Ok(authResult.Value) :  authResult.ToProblem();
        }
        [HttpPut("revoked-refresh-token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var Result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return Result.IsSuccess ? Ok() : Result.ToProblem();
        }

    }
}
