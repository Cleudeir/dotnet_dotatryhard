using System.Text.Json;
using dotatryhard.Models;
using dotatryhard.Interfaces;
using dotatryhard.Utils;
namespace dotatryhard.Services
{
    public class PlayerProfileService : ISteamUserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string? baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        private readonly string? apiKey = Environment.GetEnvironmentVariable("KEY_API");


        public PlayerProfileService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Player?> FetchProfilesAsync(long account_id)
        {
            var startTime = DateTime.Now;
            var player = new Player();
            var steamId = SteamIDConverter.ConvertAccountIDToSteamID64(account_id);

            if (account_id < 200)
            {
                player = new Player
                {
                    account_id = account_id,
                    personaname = "unknown",
                    avatarfull = null,
                    loccountrycode = "unknown"
                };
                return  player;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var response = await httpClient.GetAsync($"{baseUrl}ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}");

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<SteamUserResponse>(content);

                if (data?.response?.players?[0] != null)
                {
                    var playerData = data?.response?.players?[0];
                    player = new Player
                    {
                        account_id = account_id,
                        personaname = playerData?.personaname,
                        avatarfull = playerData?.avatarfull,
                        loccountrycode = playerData?.loccountrycode
                    };
                }
                else
                {
                    player = new Player
                    {
                        account_id = account_id,
                        personaname = "unknown",
                        avatarfull = null,
                        loccountrycode = "unknown"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching profile for accountId {account_id}: {ex.Message}");
            }

            Console.WriteLine($"Profile processing completed in {(DateTime.Now - startTime).TotalSeconds} seconds");
            return player;
        }
    }
}