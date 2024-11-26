using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotatryhard.Data;
using dotatryhard.Models;

namespace dotatryhard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersMatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayersMatchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayersMatches
        [HttpGet]
        public async Task<IActionResult> GetPlayersMatches()
        {
            var playersMatches = await _context.PlayersMatches
                .Include(pm => pm.Player)
                .Include(pm => pm.Match)
                .ToListAsync();
            return Ok(playersMatches);
        }

        // GET: api/PlayersMatches/{accountId}/{matchId}
        [HttpGet("{accountId}/{matchId}")]
        public async Task<IActionResult> GetPlayersMatch(long accountId, long matchId)
        {
            var playersMatch = await _context.PlayersMatches
                .Include(pm => pm.Player)
                .Include(pm => pm.Match)
                .FirstOrDefaultAsync(pm => pm.AccountId == accountId && pm.MatchId == matchId);

            if (playersMatch == null)
            {
                return NotFound();
            }

            return Ok(playersMatch);
        }

        // POST: api/PlayersMatches
        [HttpPost]
        public async Task<IActionResult> CreatePlayersMatch([FromBody] PlayersMatches playersMatch)
        {
            if (playersMatch == null)
            {
                return BadRequest();
            }

            _context.PlayersMatches.Add(playersMatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayersMatch), new { accountId = playersMatch.AccountId, matchId = playersMatch.MatchId }, playersMatch);
        }

        // PUT: api/PlayersMatches/{accountId}/{matchId}
        [HttpPut("{accountId}/{matchId}")]
        public async Task<IActionResult> UpdatePlayersMatch(long accountId, long matchId, [FromBody] PlayersMatches updatedPlayersMatch)
        {
            if (updatedPlayersMatch == null || accountId != updatedPlayersMatch.AccountId || matchId != updatedPlayersMatch.MatchId)
            {
                return BadRequest();
            }

            var playersMatch = await _context.PlayersMatches
                .FirstOrDefaultAsync(pm => pm.AccountId == accountId && pm.MatchId == matchId);

            if (playersMatch == null)
            {
                return NotFound();
            }

            // Update the properties
            playersMatch.Assists = updatedPlayersMatch.Assists;
            playersMatch.Deaths = updatedPlayersMatch.Deaths;
            playersMatch.Kills = updatedPlayersMatch.Kills;
            playersMatch.GoldPerMin = updatedPlayersMatch.GoldPerMin;
            playersMatch.XpPerMin = updatedPlayersMatch.XpPerMin;

            _context.PlayersMatches.Update(playersMatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PlayersMatches/{accountId}/{matchId}
        [HttpDelete("{accountId}/{matchId}")]
        public async Task<IActionResult> DeletePlayersMatch(long accountId, long matchId)
        {
            var playersMatch = await _context.PlayersMatches
                .FirstOrDefaultAsync(pm => pm.AccountId == accountId && pm.MatchId == matchId);

            if (playersMatch == null)
            {
                return NotFound();
            }

            _context.PlayersMatches.Remove(playersMatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
