using dotatryhard.Services;
using Microsoft.AspNetCore.Mvc;
using dotatryhard.Interfaces;

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

        // GET: http://localhost:5034/api/PlayersMatches/6783727637
        [HttpGet("{match_seq_number}")]
        public async Task<IActionResult> GetPlayersMatch(long match_seq_number)
        {
            MatchDetailResponse? matchDetails = await _matchDetailService.GetMatchDetailsAsync(match_seq_number);
            if (matchDetails == null)
            {
                return NotFound();
            }
            // Structure the JSON response
            var response = new
            {
                match = matchDetails.Match,
                players = matchDetails.Players,
                playersMatches = matchDetails.PlayersMatches                
            };
            return Ok(matchDetails);
        }

    }
}
