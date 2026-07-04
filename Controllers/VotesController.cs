
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Contracts.Votes;


namespace SurveyBasket.Controllers
{
    [Route("api/Polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Member)]
    [EnableRateLimiting("concurrencyLimit")]
    public class VotesController(IQuestionService questionService,IVoteService voteService) : ControllerBase
    {
        private readonly  IQuestionService _QuestionService = questionService;
        private readonly IVoteService _voteService = voteService;

        [HttpGet("start")]
      
        public async Task<IActionResult> StartVote([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _QuestionService.GetAvliableAsync(pollId, userId!, cancellationToken);
            if (result.IsSuccess) return Ok(result.Value);

            return result.ToProblem();


        }
        [HttpPost("")]
        public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request ,CancellationToken cancallationToken)
        {
            var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancallationToken);
            if (result.IsSuccess) return Created();

                 return result.ToProblem();

        }
    }
}
