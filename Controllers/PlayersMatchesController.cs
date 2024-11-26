using dotatryhard.Data;
using dotatryhard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var playersMatches = await _context
                .players_matches.Include(pm => pm.player)
                .Include(pm => pm.match)
                .ToListAsync();
            return Ok(playersMatches);
        }

        // GET: api/PlayersMatches/{accountId}/{matchId}
        [HttpGet("{accountId}/{matchId}")]
        public async Task<IActionResult> GetPlayersMatch(long accountId, long matchId)
        {
            var playersMatch = await _context
                .players_matches.Include(pm => pm.player)
                .Include(pm => pm.match)
                .FirstOrDefaultAsync(pm => pm.account_id == accountId && pm.match_id == matchId);

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

            _context.players_matches.Add(playersMatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPlayersMatch),
                new { accountId = playersMatch.account_id, matchId = playersMatch.match_id },
                playersMatch
            );
        }

        // PUT: api/PlayersMatches/{accountId}/{matchId}
        [HttpPut("{accountId}/{matchId}")]
        public async Task<IActionResult> UpdatePlayersMatch(
            long accountId,
            long matchId,
            [FromBody] PlayersMatches updatedPlayersMatch
        )
        {
            if (
                updatedPlayersMatch == null
                || accountId != updatedPlayersMatch.account_id
                || matchId != updatedPlayersMatch.match_id
            )
            {
                return BadRequest();
            }

            var playersMatch = await _context.players_matches.FirstOrDefaultAsync(pm =>
                pm.account_id == accountId && pm.match_id == matchId
            );

            if (playersMatch == null)
            {
                return NotFound();
            }

            // Update the properties
            playersMatch.assists = updatedPlayersMatch.assists;
            playersMatch.deaths = updatedPlayersMatch.deaths;
            playersMatch.kills = updatedPlayersMatch.kills;
            playersMatch.gold_per_min = updatedPlayersMatch.gold_per_min;
            playersMatch.xp_per_min = updatedPlayersMatch.xp_per_min;

            _context.players_matches.Update(playersMatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/PlayersMatches/{accountId}/{matchId}
        [HttpDelete("{accountId}/{matchId}")]
        public async Task<IActionResult> DeletePlayersMatch(long accountId, long matchId)
        {
            var playersMatch = await _context.players_matches.FirstOrDefaultAsync(pm =>
                pm.account_id == accountId && pm.match_id == matchId
            );

            if (playersMatch == null)
            {
                return NotFound();
            }

            _context.players_matches.Remove(playersMatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
