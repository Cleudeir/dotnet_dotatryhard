using System.Text.Json;
using dotatryhard.Interfaces;
using dotatryhard.Models;

public class MatchDetailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public MatchDetailService( IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MatchDetailResponse?> GetMatchDetailsAsync(long match_seq_number)
    {
        var startTime = DateTime.Now;
        var playersUnique = new HashSet<long>();
        var matches = new List<Match>();
        var playersMatches = new List<PlayersMatches>();
        long matchesGameModeError = 0;

        try
        {
            Console.WriteLine($"Processing matchDetails {match_seq_number}");

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{Environment.GetEnvironmentVariable("BASE_URL")}/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?matches_requested=1&start_at_match_seq_num={match_seq_number}&key={Environment.GetEnvironmentVariable("KEY_API")}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var matchesData = JsonSerializer.Deserialize<DotaGetMatchHistoryBySequenceNumResponse>(content);
            if (matchesData == null)
            {
                matchesGameModeError = match_seq_number;
                return null;
            }
            var match = matchesData.result.matches[0];
            if (matchesData?.result == null && match?.game_mode != 18 && match == null)
            {
                matchesGameModeError = match_seq_number;
                return null;
            }
            // Process match
            matches.Add(new Match
            {
                match_id = match.match_id,
                start_time = match.start_time,
                match_seq_num = match.match_seq_num,
                cluster = match.cluster,
                dire_score = match.dire_score,
                radiant_score = match.radiant_score,
                duration = match.duration,
                radiant_win = match.radiant_win
            });
            // Process players
            foreach (var player in match.players)
            {
                var uniqueAbility = new HashSet<int>();
                if (player.ability_upgrades != null)
                {
                    foreach (var ability in player.ability_upgrades)
                    {
                        uniqueAbility.Add(ability.ability);
                    }
                }
                var abilities = uniqueAbility.ToArray();
                var isWin = player.player_slot < 5 == match.radiant_win ? 1 : 0;
                var score = player.assists * 1 - player.deaths * 1 + player.denies * 1 + player.denies * 1 + player.gold_per_min * 1;
                var account_id = player.account_id == 4294967295 ? player.player_slot + 1 : player.account_id;
                playersMatches.Add(new PlayersMatches
                {
                    account_id = account_id,
                    match_id = match.match_id,
                    assists = player.assists,
                    deaths = player.deaths,
                    denies = player.denies,
                    gold_per_min = player.gold_per_min,
                    hero_damage = player.hero_damage,
                    hero_healing = player.hero_healing,
                    kills = player.kills,
                    last_hits = player.last_hits,
                    net_worth = player.net_worth,
                    tower_damage = player.tower_damage,
                    xp_per_min = player.xp_per_min,
                    win = (byte)isWin,
                    ability_0 = abilities.ElementAtOrDefault(0),
                    ability_1 = abilities.ElementAtOrDefault(1),
                    ability_2 = abilities.ElementAtOrDefault(2),
                    ability_3 = abilities.ElementAtOrDefault(3),
                    hero_level = player.level,
                    team = player.team_number,
                    leaver_status = player.leaver_status,
                    aghanims_scepter = player.aghanims_scepter,
                    aghanims_shard = player.aghanims_shard,
                    backpack_0 = player.backpack_0,
                    backpack_1 = player.backpack_1,
                    backpack_2 = player.backpack_2,
                    hero_id = player.hero_id,
                    moonshard = player.moonshard,
                    item_neutral = player.item_neutral,
                    item_0 = player.item_0,
                    item_1 = player.item_1,
                    item_2 = player.item_2,
                    item_3 = player.item_3,
                    item_4 = player.item_4,
                    item_5 = player.item_5,
                    player_slot = player.player_slot,
                    score = score,
                });
                playersUnique.Add(account_id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing matchDetails for {match_seq_number}: {ex.Message}");
        }

        Console.WriteLine($"Completed in {(DateTime.Now - startTime).TotalSeconds} seconds");
        return new MatchDetailResponse { Matches = matches, PlayersMatches = playersMatches, PlayersUnique = playersUnique, MatchesGameModeError = matchesGameModeError };
    }
}

public class MatchDetailResponse
{
    public required List<Match> Matches { get; set; }
    public required List<PlayersMatches> PlayersMatches { get; set; }
    public required HashSet<long>? PlayersUnique { get; set; }
    public required long? MatchesGameModeError { get; set; }
}

