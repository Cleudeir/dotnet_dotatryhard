using dotatryhard.Interfaces;
using dotatryhard.Models;
using dotatryhard.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotatryhard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersMatchesController : ControllerBase
    {
        private readonly PlayersMatchesService _matchDetailService;

        public PlayersMatchesController(PlayersMatchesService matchDetailService)
        {
            _matchDetailService = matchDetailService;
        }

        // GET: http://localhost:5034/api/PlayersMatches/6783727637
        [HttpGet("{match_seq_number}")]
        public async Task<IActionResult> GetPlayersMatch(long match_seq_number)
        {
            MatchDetailResponse? matchDetails = await _matchDetailService.GetMatchDetailsAsync(
                match_seq_number
            );
            if (matchDetails == null)
            {
                return NotFound();
            }
            return Ok(matchDetails);
        }

        // GET: http://localhost:5034/api/PlayersMatches/average
        [HttpGet("average")]
        public async Task<IActionResult> GetAverageData()
        {
            try
            {
                var averageData = await _matchDetailService.GetAllWithAveragesAsync();
                return Ok(averageData);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        message = "An error occurred while processing the request.",
                        details = ex.Message,
                    }
                );
            }
        }
    }
}
