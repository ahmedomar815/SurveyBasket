
using Microsoft.AspNetCore.Identity;


namespace SurveyBasket.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    
    public class PollsController(IPollService pollService,UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private  readonly IPollService  _pollService= pollService;

        public UserManager<ApplicationUser> _UserManager  = userManager;


        [HttpGet("getAll")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetAllAsync(cancellationToken)); 
        }
        [HttpGet("current")]
        [Authorize(Roles =DefaultRoles.Member)]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
           
            return Ok(await _pollService.GetCurrentAsync( cancellationToken));
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> Get([FromRoute] int Id, CancellationToken cancellationToken)
        {
            var result = await _pollService.GetAsync(Id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value.Adapt<PollResponse>()) : result.ToProblem();

        }
       
         [HttpPost("")]
        [HasPermission(Permissions.AddPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest request,CancellationToken cancellationToken)
         {
            var result = await _pollService.AddAsync(request, cancellationToken);

            if (result.IsFailure)
                return result.ToProblem();

            var poll = result.Value;

            return CreatedAtAction(  nameof(Get),  new { Id = poll.Id },   poll.Adapt<PollResponse>());

        }

        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PollRequest request, CancellationToken cancellationToken)
        {
           var result = await _pollService.UpdateAsync(Id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
                 
        }
        [HttpDelete("{Id}")]
        [HasPermission(Permissions.DeletePolls)]
        public async Task< IActionResult> Delete([FromRoute] int Id,CancellationToken cancellationToken)
        {
            var result = await  _pollService.DeleteAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{Id}/togglePublish")]
        [HasPermission(Permissions.UpdatePolls)]
        public async Task<IActionResult> TogglePublish([FromRoute] int Id, CancellationToken cancellationToken)
        {
            var result = await _pollService.TogglePublishStatusAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();

        }

    }
}
 