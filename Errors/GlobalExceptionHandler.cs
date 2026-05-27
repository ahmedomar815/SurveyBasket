using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.Errors
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> Logger) : IExceptionHandler
    {
        public ILogger<GlobalExceptionHandler> _Logger  = Logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _Logger.LogError(exception, "Something went wrong :{Message}", exception.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "internal Server Error ",
                Type= "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            };
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
