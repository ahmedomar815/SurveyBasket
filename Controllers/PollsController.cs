using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Entities;
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
            var polls = await _pollService.GetAllAsync(cancellationToken);
            var response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(response);

           

        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
        {
           var currentPoll=await _pollService.GetAsync(Id, cancellationToken);
            if(currentPoll is null) return NotFound();
            var response= currentPoll.Adapt<PollResponse>();
            return Ok(response);
        }
       
         [HttpPost("")]
         public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
         {
             var poll=await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
             return CreatedAtAction(nameof(Get), new { Id = poll.Id }, poll.Adapt<PollResponse>());
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
           var Isupdated= await _pollService.UpdateAsync(Id, request.Adapt<Poll>(), cancellationToken);
            if (!Isupdated)
                return NotFound();
            return NoContent();
        }
        [HttpDelete("{Id}")]
        public async Task< IActionResult> Delete([FromRoute] int Id,CancellationToken cancellationToken)
        {
            var IsDelete = await  _pollService.DeleteAsync(Id, cancellationToken);
            if (!IsDelete)
                return NotFound();
            return NoContent();
        }
        [HttpPut("{Id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int Id, CancellationToken cancellationToken)
        {
            var IsUpdated = await _pollService.TogglePublishStatusAsync(Id, cancellationToken);
            if (!IsUpdated)
                return NotFound();
            return NoContent();
        }

    }
}
