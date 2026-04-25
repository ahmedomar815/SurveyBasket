
using SurveyBasket.Services;
using System.Runtime.CompilerServices;

namespace SurveyBasket.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private  readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet("getAll")] 
        public IActionResult GetAll()
        {
            return Ok(_pollService.GetAll());
        }
        [HttpGet("{Id}")]
        public IActionResult Get(int Id)
        {
            var poll = _pollService.Get(Id);
          return poll is null ? NotFound() : Ok(poll); 
        }
        [HttpPost("")]
        public IActionResult Add(Poll request)
        {
            return CreatedAtAction(nameof(Get),new {id=request.Id},request);
        }
        [HttpPut("")]
        public IActionResult Update(int Id,Poll request)
        {
            var IsUpdated = _pollService.Update(Id, request);
            if (!IsUpdated)
                return NotFound();
            return NoContent();


        }
        [HttpDelete("")]
        public IActionResult Delete(int Id)
        {
            var IsDelete = _pollService.Delete(Id);
            if (!IsDelete)
                return NotFound();
            return NoContent();
        }
    }
}
