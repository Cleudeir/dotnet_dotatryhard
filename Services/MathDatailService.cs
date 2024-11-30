using System.Text.Json;
using dotatryhard.Data;
using dotatryhard.Models;
using Microsoft.EntityFrameworkCore;

public class MatchDetailService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _userHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public MatchDetailService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MatchDetailResponse?> GetMatchDetailsAsync(List<long> match_seq_numbers)
    {
        var startTime = DateTime.Now;
        var matches = new List<Match>();
        var playersMatches = new List<PlayersMatches>();
        var playerUnique = new HashSet<string>();

        // Check existing matches in the database
        var existingMatchIds = await _context.PlayersMatches
            .Where(pm => match_seq_numbers.Contains(pm.match_id))
            .Select(pm => pm.match_id)
            .ToListAsync();

        var filtered_match_seq_numbers = match_seq_numbers.Except(existingMatchIds).ToList();
        if (filtered_match_seq_numbers.Count == 0) return null;

        // Load previously errored matches
        var matchesGameModeErrorPath = Path.Combine(_userHomeDir, "temp", "matchesGameModeError.json");
        var matchesGameModeError = LoadMatchesGameModeError(matchesGameModeErrorPath);

        foreach (var match_seq_number in filtered_match_seq_numbers)
        {
            if (matchesGameModeError.Contains(match_seq_number)) continue;

            try
            {
                Console.WriteLine($"Processing matchDetails {match_seq_number}");
                var matchData = await FetchMatchDetailsAsync(match_seq_number);
                if (matchData?.Result == null || matchData.Result.GameMode != 18)
                {
                    matchesGameModeError.Add(match_seq_number);
                    continue;
                }

                // Process match
                matches.Add(new Match
                {
                    MatchId = matchData.Result.MatchId,
                    StartTime = matchData.Result.StartTime,
                    Cluster = Regions.GetRegion(matchData.Result.Cluster),
                    DireScore = matchData.Result.DireScore,
                    RadiantScore = matchData.Result.RadiantScore,
                    Duration = matchData.Result.Duration
                });

                // Process players
                foreach (var player in matchData.Result.Players)
                {
                    var uniqueAbility = new HashSet<int>();
                    if (player.AbilityUpgrades != null)
                    {
                        foreach (var ability in player.AbilityUpgrades)
                        {
                            uniqueAbility.Add(ability.AbilityId);
                        }
                    }

                    var abilities = uniqueAbility.ToArray();
                    var isWin = player.PlayerSlot < 5 == matchData.Result.RadiantWin;

                    playersMatches.Add(new PlayersMatches
                    {
                        AccountId = player.AccountId == 4294967295 ? player.PlayerSlot + 1 : player.AccountId,
                        MatchId = matchData.Result.MatchId,
                        Assists = player.Assists,
                        Deaths = player.Deaths,
                        Denies = player.Denies,
                        GoldPerMin = player.GoldPerMin,
                        HeroDamage = player.HeroDamage,
                        HeroHealing = player.HeroHealing,
                        Kills = player.Kills,
                        LastHits = player.LastHits,
                        NetWorth = player.NetWorth,
                        TowerDamage = player.TowerDamage,
                        XpPerMin = player.XpPerMin,
                        Win = isWin ? 1 : 0,
                        Ability0 = Ability.GetAbility(abilities.ElementAtOrDefault(0)),
                        Ability1 = Ability.GetAbility(abilities.ElementAtOrDefault(1)),
                        Ability2 = Ability.GetAbility(abilities.ElementAtOrDefault(2)),
                        Ability3 = Ability.GetAbility(abilities.ElementAtOrDefault(3)),
                        HeroLevel = player.Level,
                        Team = player.TeamNumber,
                        LeaverStatus = player.LeaverStatus
                    });

                    playerUnique.Add(JsonSerializer.Serialize(new
                    {
                        LocCountryCode = Regions.GetRegion(matchData.Result.Cluster),
                        AccountId = player.AccountId == 4294967295 ? player.PlayerSlot + 1 : player.AccountId
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing matchDetails for {match_seq_number}: {ex.Message}");
            }
        }

        // Save errored matches
        SaveMatchesGameModeError(matchesGameModeErrorPath, matchesGameModeError);

        // Save data to the database
        await _context.Match.AddRangeAsync(matches);
        await _context.PlayersMatches.AddRangeAsync(playersMatches);
        await _context.SaveChangesAsync();

        foreach (var playerJson in playerUnique)
        {
            var playerData = JsonSerializer.Deserialize<PlayerData>(playerJson);
            await _context.Player
                .Where(p => p.AccountId == playerData.AccountId)
                .UpdateAsync(p => new Player
                {
                    LocCountryCode = playerData.LocCountryCode
                });
        }

        Console.WriteLine($"Completed in {(DateTime.Now - startTime).TotalSeconds} seconds");
        return new MatchDetailResponse { Matches = matches, PlayersMatches = playersMatches };
    }

    private async Task<DotaGetMatchHistoryBySequenceNumResponse> FetchMatchDetailsAsync(long match_seq_number)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?matches_requested=1&start_at_match_seq_num={match_seq_number}&key={Environment.GetEnvironmentVariable("KEY_API")}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<DotaGetMatchHistoryBySequenceNumResponse>(content);
    }

    private HashSet<long> LoadMatchesGameModeError(string filePath)
    {
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<HashSet<long>>(content) ?? new HashSet<long>();
        }
        return new HashSet<long>();
    }

    private void SaveMatchesGameModeError(string filePath, HashSet<long> matchesGameModeError)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, JsonSerializer.Serialize(matchesGameModeError));
    }
}

public class MatchDetailResponse
{
    public List<Match> Matches { get; set; }
    public List<PlayersMatches> PlayersMatches { get; set; }
}

public class PlayerData
{
    public string LocCountryCode { get; set; }
    public long AccountId { get; set; }
}
