using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Abstractions;
using SurveyBasket.Entities;
using SurveyBasket.Errors;
using SurveyBasket.Services;
using System.Collections.Generic;


namespace SurveyBasket.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPollService pollService,UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private  readonly IPollService  _pollService= pollService;

        public UserManager<ApplicationUser> _UserManager  = userManager;

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetAllAsync(cancellationToken)); 
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
           
            return Ok(await _pollService.GetCurrentAsync( cancellationToken));
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
        {
            var result = await _pollService.GetAsync(Id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value.Adapt<PollResponse>()) : result.ToProblem();

        }
       
         [HttpPost("")]
         public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
         {
            var result = await _pollService.AddAsync(request, cancellationToken);

            if (result.IsFailure)
                return result.ToProblem();

            var poll = result.Value;

            return CreatedAtAction(  nameof(Get),  new { Id = poll.Id },   poll.Adapt<PollResponse>());

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
           var result = await _pollService.UpdateAsync(Id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
                 
        }
        [HttpDelete("{Id}")]
        public async Task< IActionResult> Delete([FromRoute] int Id,CancellationToken cancellationToken)
        {
            var result = await  _pollService.DeleteAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{Id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int Id, CancellationToken cancellationToken)
        {
            var result = await _pollService.TogglePublishStatusAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();

        }

    }
}
 