using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Contracts.Validations;
using SurveyBasket.Services;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace SurveyBasket.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private  readonly IPollService  _pollService= pollService;
        

        [HttpGet("getAll")] 
        public IActionResult GetAll()
        {
            var polls= _pollService.GetAll();
            var response=polls.Adapt<IEnumerable<Poll>>();  

            return Ok(response);
        }
        [HttpGet("{Id}")]
        public IActionResult Get([FromRoute]int Id)
        {
            var poll = _pollService.Get(Id);
            var response = poll.Adapt<PollResponse>();

            return poll is null ? NotFound() : Ok(response); 
        }
        [HttpPost("")]
        public IActionResult Add([FromBody] createPollRequest request,
            [FromServices] IValidator<createPollRequest> validator)
        {
          
            var newpoll = _pollService.Add(request.Adapt<Poll>());
            return CreatedAtAction(nameof(Get),new {id= newpoll.Id}, newpoll);
        }
        [HttpPut("{Id}")]
        public IActionResult Update([FromRoute] int Id, [FromBody] createPollRequest request)
        {
            var IsUpdated = _pollService.Update(Id, request.Adapt<Poll>());
            if (!IsUpdated)
                return NotFound();
            return NoContent();


        }
        [HttpDelete("{Id}")]
        public IActionResult Delete([FromRoute] int Id)
        {
            var IsDelete = _pollService.Delete(Id);
            if (!IsDelete)
                return NotFound();
            return NoContent();
        }
      

    }
}
