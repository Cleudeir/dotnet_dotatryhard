using Microsoft.AspNetCore.Mvc;
using dotatryhard.Data;
using dotatryhard.Models;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            return Ok(await _context.Players.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] Player player)
        {
            if (player == null) return BadRequest();

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            return Ok(player);
        }
    }
}
