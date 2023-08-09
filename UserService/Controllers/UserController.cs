using UserService.Services;
using UserService.Models;
using Microsoft.AspNetCore.Mvc;
using static UserService.Data.UsersData;

namespace UserService.Controllers
{
    [Route("/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IVotingService _votingService;

        public UserController(IVotingService votingService)
        {
            _votingService = votingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            List<Vote> votes = await _votingService.GetVoteByUserId(id);

            if (votes != null)
                return Ok(new { id = user.Id, name = user.Name, votes });

            return Ok(user);
        }
    }
}