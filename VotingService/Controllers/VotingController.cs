using Microsoft.AspNetCore.Mvc;
using static VotingService.Data.VotingData;

namespace VotingService.Controllers
{
    [Route("/")]
    [ApiController]
    public class VotingController : ControllerBase
    {
        private static int requestCounter = 1;

        [HttpGet("{id}")]
        public IActionResult GetVoteByUserId(int id)
        {
            requestCounter++;

            // Testing retry
            if (id == 1 && requestCounter % 2 != 0)
            {
                return StatusCode(504, "Gateway Timeout");
            }

            // Testing circuit breaking
            if (id == 2)
            {
                return StatusCode(500, "Internal Server Error");
            }

            var votes = Votes.Where(v => v.UserId == id);
            if (votes == null)
            {
                return NotFound();
            }

            return Ok(votes);
        }
    }
}