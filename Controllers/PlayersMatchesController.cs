using Microsoft.AspNetCore.Mvc;

namespace dotatryhard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersMatchesController : ControllerBase
    {
        private readonly MatchDetailService _matchDetailService;

        public PlayersMatchesController(MatchDetailService matchDetailService)
        {
            _matchDetailService = matchDetailService;
        }
        
        // GET: api/PlayersMatches/{match_seq_number}
        [HttpGet("{match_seq_number}")]
        public async Task<IActionResult> GetPlayersMatch(long match_seq_number)
        {
            MatchDetailResponse? matchDetails = await _matchDetailService.GetMatchDetailsAsync(match_seq_number);
            if (matchDetails == null)
            {
                return NotFound();
            }

            return Ok(matchDetails);
        }

    }
}
