using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchHistoryController : ControllerBase
    {
        private readonly IMatchHistoryService _matchHistoryService;

        public MatchHistoryController(IMatchHistoryService matchHistoryService)
        {
            _matchHistoryService = matchHistoryService;
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetMatchHistory(int accountId)
        {
            var result = await _matchHistoryService.GetMatchHistoryAsync(accountId);
            if (result == null)
            {
                return NotFound(new { message = "No new match history found." });
            }

            // Structure the JSON response
            var response = new
            {
                Matches = result.Value.Matches, // List of match IDs
                Players = result.Value.Players, // List of player IDs
            };

            return Ok(response);
        }
    }
}
