using dotatryhard.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotatryhard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ISteamUserService _playerProfileResponse;

        public PlayerController(ISteamUserService playerProfileResponse)
        {
            _playerProfileResponse = playerProfileResponse;
        }

        // http://localhost:5034/api/Player/87683422
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetPlayer(long accountId)
        {
            var result = await _playerProfileResponse.FetchProfilesAsync(accountId);
            if (result == null)
            {
                return NotFound(new { message = "No new match history found." });
            }
            // Structure the JSON response
            var response = new
            {
                account_id = result.account_id,
                personaname = result.personaname,
                avatarfull = result.avatarfull,
                loccountrycode = result.loccountrycode,
            };
            return Ok(response);
        }
    }
}
