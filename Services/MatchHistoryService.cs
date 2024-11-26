using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using dotatryhard.Data;
using dotatryhard.Interfaces;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Services
{
    public class MatchHistoryService : IMatchHistoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public MatchHistoryService(
            HttpClient httpClient,
            ApplicationDbContext dbContext,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<(List<int> Matches, List<int> Players)?> GetMatchHistoryAsync(
            int accountId
        )
        {
            var startTime = DateTime.Now;

            try
            {
                // Construct API URL using configuration
                var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
                var gameMode = Environment.GetEnvironmentVariable("GAME_MODE");
                var apiKey = Environment.GetEnvironmentVariable("KEY_API2");
                var url =
                    $"{baseUrl}IDOTA2Match_570/GetMatchHistory/v1/?account_id={accountId}&game_mode={gameMode}&key={apiKey}";

                // Fetch match history from API
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();

                // debug response
                Console.WriteLine(responseData);

                // Deserialize response JSON
                var data = JsonSerializer.Deserialize<DotaMatchHistoryResponse>(responseData);
                // debug response
                Console.WriteLine(data);

                if (data?.Result?.Matches == null)
                {
                    return null;
                }

                // Retrieve existing match IDs from database
                var matchIds = data.Result.Matches.Select(m => m.MatchId).ToList();
                var existingMatchIds = await _dbContext
                    .PlayersMatches.Where(pm => matchIds.Contains(pm.MatchId))
                    .Select(pm => pm.MatchId)
                    .ToListAsync();

                // Filter out matches that already exist in the database
                var newMatches = data
                    .Result.Matches.Where(m => !existingMatchIds.Contains(m.MatchId))
                    .ToList();

                if (!newMatches.Any())
                {
                    return null;
                }

                // Collect unique match IDs and player IDs
                var matchesSet = new HashSet<long>();
                var playersSet = new HashSet<int>();

                foreach (var match in newMatches)
                {
                    matchesSet.Add(match.MatchId);
                    foreach (var player in match.Players)
                    {
                        int playerAccountId =
                            player.AccountId == 4294967295
                                ? player.PlayerSlot + 1
                                : player.AccountId;
                        playersSet.Add(playerAccountId);
                    }
                }

                // Convert sets to lists
                var matches = matchesSet.Select(x => (int)x).ToList();
                var players = playersSet.ToList();

                Console.WriteLine(
                    $"Match history processed in {(DateTime.Now - startTime).TotalSeconds} seconds."
                );
                return (matches, players);
            }
            catch (Exception ex)
            {
                Console.WriteLine("<<<<<< matchHistory-error >>>>>>>");
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}

// Models for API response deserialization
public class DotaMatchHistoryResponse
{
    public Result Result { get; set; }
}

public class Result
{
    public List<Match> Matches { get; set; }
}

public class Match
{
    public long MatchId { get; set; }
    public List<Player> Players { get; set; }
}

public class Player
{
    public int AccountId { get; set; }
    public int PlayerSlot { get; set; }
}
