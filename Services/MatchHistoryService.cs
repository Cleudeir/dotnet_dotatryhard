using System.Text.Json;
using dotatryhard.Data;
using dotatryhard.Interfaces;

namespace dotatryhard.Services
{
    public class MatchHistoryService : IMatchHistoryService
    {
        private readonly HttpClient _httpClient;

        public MatchHistoryService(
            HttpClient httpClient,
            ApplicationDbContext dbContext,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
        }

        public async Task<(List<MatchInfo> Matches, List<int> Players)?> GetMatchHistoryAsync(int accountId)
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
                var data = JsonSerializer.Deserialize<DotaMatchHistoryResponse>(responseData);

                if (data?.result?.matches == null)
                {
                    return null;
                }

                // Filter out matches that already exist in the database
                var newMatches = data.result.matches;
                // Collect unique match IDs and player IDs
                var matchesSet = new HashSet<MatchInfo>();
                var playersSet = new HashSet<int>();

                foreach (var match in newMatches)
                {
                    matchesSet.Add(new MatchInfo
                    {
                        id = match.match_id,
                        seq_num = match.match_seq_num
                    });
                    foreach (var player in match.players)
                    {
                        int playerAccountId = (int)(
                            player.account_id == 4294967295
                                ? player.player_slot + 1
                                : player.account_id
                        );
                        playersSet.Add(playerAccountId);
                    }
                }

                // Convert sets to lists

                var players = playersSet.ToList();
                var matches = matchesSet.ToList();

                Console.WriteLine($"Found {matches.Count} new matches.");
                Console.WriteLine($"Found {players.Count} new players.");

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

