using System.Text.Json;
using dotatryhard.Interfaces;
using dotatryhard.Models;

public class MatchDetailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public MatchDetailService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MatchDetailResponse?> GetMatchDetailsAsync(long match_seq_number)
    {
        var startTime = DateTime.Now;
        var players = new HashSet<long>();
        Match newMatch = new();
        var playersMatches = new List<PlayersMatches>();

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
                return null;
            }
            var match = matchesData.result.matches[0];
            if (matchesData?.result == null && match?.game_mode != 18 && match == null)
            {
                return null;
            }
            // Process match
            newMatch = new Match
            {
                match_id = match.match_id,
                start_time = match.start_time,
                match_seq_num = match.match_seq_num,
                cluster = match.cluster,
                dire_score = match.dire_score,
                radiant_score = match.radiant_score,
                duration = match.duration,
                radiant_win = match.radiant_win
            };
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
                var win = player.player_slot < 5 == match.radiant_win ? 1 : 0;
                var account_id = player.account_id == 4294967295 ? player.player_slot + 1 : player.account_id;
                var weight = new
                {
                    assists = 5,
                    last_hits = 1,
                    denies = -1,
                    kills = 1,
                    deaths = 1,
                    gold_per_min = 1,
                    hero_damage = 1,
                    hero_healing = 0.1,
                    net_worth = 1,
                    tower_damage = 1,
                    xp_per_min = 1,
                    winRate = 5,
                };

                var totalWeight = weight.assists +
                  weight.last_hits +
                  weight.denies +
                  weight.kills +
                  weight.deaths +
                  weight.gold_per_min +
                  weight.hero_damage +
                  weight.hero_healing +
                  weight.net_worth +
                  weight.tower_damage +
                  weight.xp_per_min +
                  weight.winRate;



                var score =
                  (player.assists * weight.assists +
                  player.last_hits * weight.last_hits +
                  player.denies * weight.denies +
                  player.kills * weight.kills +
                  player.deaths * weight.deaths +
                  player.gold_per_min * weight.gold_per_min +
                  player.hero_damage * weight.hero_damage +
                  player.hero_healing * weight.hero_healing +
                  player.net_worth * weight.net_worth +
                  player.tower_damage * weight.tower_damage +
                  player.xp_per_min * weight.xp_per_min +
                  win * weight.winRate) / totalWeight;


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
                    win = (byte)win,
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
                    score = (int?)score,
                });
                players.Add(account_id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing matchDetails for {match_seq_number}: {ex.Message}");
        }

        Console.WriteLine($"Completed in {(DateTime.Now - startTime).TotalSeconds} seconds");
        return new MatchDetailResponse { Match = newMatch, PlayersMatches = playersMatches, Players = players };
    }
}

public class MatchDetailResponse
{
    public required Match Match { get; set; }
    public required List<PlayersMatches> PlayersMatches { get; set; }
    public required HashSet<long>? Players { get; set; }
}

