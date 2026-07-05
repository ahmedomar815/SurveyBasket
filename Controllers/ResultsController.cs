
using Asp.Versioning;

namespace SurveyBasket.Controllers
{
    [ApiVersion(1, Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/polls/{pollId}/{controller}")]
    [HasPermission(Permissions.Results)]
    public class ResultsController(IResultService resultService): ControllerBase
    {
        public IResultService _resultService= resultService;

        [HttpGet("row-data")]
        public async Task<IActionResult> PollVotes([FromRoute] int pollId, CancellationToken cancallationToken )
        {
            var result = await _resultService.GetPollVotesResultAsync(pollId, cancallationToken);
            return result.IsSuccess? Ok(result.Value): result.ToProblem();
        }
        [HttpGet("votes-perday")]
        public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancallationToken)
        {
            var result = await _resultService.GetVotesPerDayAsync(pollId, cancallationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("votes-per-Question")]
        public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancallationToken)
        {
            var result = await _resultService.GetVotePerQuestionAsync(pollId, cancallationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }

}
