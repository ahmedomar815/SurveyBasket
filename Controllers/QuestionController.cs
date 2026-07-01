using SurveyBasket.Contracts.Commons;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Controllers
{
    [Route("api/Polls/{pollId}/[controller]")]
    [ApiController]
    
    public class QuestionController(IQuestionService questionService) : ControllerBase
    {
        public readonly IQuestionService _QuestionService = questionService;
     
        [HttpGet("")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, [FromQuery]RequestFilter filter, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.GetAllAsync(pollId, filter,cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("{questionId}")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> Get([FromRoute]int pollId, [FromRoute] int questionId, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.GetAsync(pollId, questionId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.AddQuestions)]
        public async Task<IActionResult> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.AddAsync(pollId, request, cancellationToken);
            if (result.IsSuccess) return CreatedAtAction(nameof(Get), new { pollId = pollId, questionId = result.Value.Id }, result.Value);

            return result.ToProblem();
        }

        [HttpPut("{questionId}")]
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int pollId, [FromRoute] int questionId,[FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result= await _QuestionService.UpdateAsync(pollId, questionId, request, cancellationToken);
            if (result.IsSuccess) return NoContent();
            return result.ToProblem();



        }
        [HasPermission(Permissions.UpdateQuestions)]
        [HttpPut("{questionId}/toggle-status")]
        public async Task<IActionResult> ToggleStatusAsync([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.ToggleStatusAsync(pollId, questionId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();

        }

    }
}
