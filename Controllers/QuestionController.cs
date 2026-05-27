using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController(IQuestionService questionService) : ControllerBase
    {
        public readonly IQuestionService _QuestionService = questionService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.GetAllAsync(pollId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute]int pollId, [FromRoute] int questionId, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.GetAsync(pollId, questionId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem(StatusCodes.Status404NotFound);
        }
        [HttpPost("")]
        public async Task<IActionResult> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.AddAsync(pollId, request, cancellationToken);
            if (result.IsSuccess) return CreatedAtAction(nameof(Get), new { pollId = pollId, Id = result.Value.Id }, result.Value);

            return result.Error.Equals(QuestionErrors.DuplicatedQuestonContent) ?
                result.ToProblem(StatusCodes.Status409Conflict) : result.ToProblem(StatusCodes.Status404NotFound);
        }

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int pollId, [FromRoute] int questionId,[FromBody] QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var result= await _QuestionService.UpdateAsync(pollId, questionId, request, cancellationToken);
            if (result.IsSuccess) return NoContent();

            return result.Error.Equals(QuestionErrors.DuplicatedQuestonContent) ?
                result.ToProblem(StatusCodes.Status409Conflict) : result.ToProblem(StatusCodes.Status404NotFound);

        }
        [HttpPut("{questionId}/toggle-status")]
        public async Task<IActionResult> ToggleStatusAsync([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken = default)
        {
            var result = await _QuestionService.ToggleStatusAsync(pollId, questionId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);
        }

    }
}
