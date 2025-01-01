using System.Text.Json;
using dotatryhard.Data;
using dotatryhard.Interfaces;
using dotatryhard.Models;
using dotatryhard.Utils;
using Microsoft.EntityFrameworkCore;

namespace dotatryhard.Services
{
    public class PlayersMatchesService : IMatchDetailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _dbContext;

        public PlayersMatchesService(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext dbContext
        )
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
        }

        public async Task<MatchDetailResponse?> GetMatchDetailsAsync(long match_seq_number)
        {
            var startTime = DateTime.Now;
            var players = new HashSet<long>();
            Match newMatch = new();
            var playersMatches = new List<PlayersMatches>();
            var playersMatchesAverages = new List<PlayersMatchesAverages>();
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(
                    $"{Environment.GetEnvironmentVariable("BASE_URL")}/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?matches_requested=1&start_at_match_seq_num={match_seq_number}&key={Environment.GetEnvironmentVariable("KEY_API2")}"
                );
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var matchesData =
                    JsonSerializer.Deserialize<DotaGetMatchHistoryBySequenceNumResponse>(content);
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
                    radiant_win = match.radiant_win,
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
                    var account_id =
                        player.account_id == 4294967295
                            ? player.player_slot + 1
                            : player.account_id;

                    playersMatches.Add(
                        new PlayersMatches
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
                        }
                    );
                    players.Add(account_id);
                    // set average
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

                    var totalWeight =
                        weight.assists
                        + weight.last_hits
                        + weight.denies
                        + weight.kills
                        + weight.deaths
                        + weight.gold_per_min
                        + weight.hero_damage
                        + weight.hero_healing
                        + weight.net_worth
                        + weight.tower_damage
                        + weight.xp_per_min
                        + weight.winRate;

                    var score_avg =
                        (
                            player.assists * weight.assists
                            + player.last_hits * weight.last_hits
                            + player.denies * weight.denies
                            + player.kills * weight.kills
                            - player.deaths * weight.deaths
                            + player.gold_per_min * weight.gold_per_min
                            + player.hero_damage * weight.hero_damage
                            + player.hero_healing * weight.hero_healing
                            + player.net_worth * weight.net_worth
                            + player.tower_damage * weight.tower_damage
                            + player.xp_per_min * weight.xp_per_min
                            + win * weight.winRate
                        )
                        / totalWeight
                        / match.duration
                        * 3000;

                    var playersMatchAverage = new PlayersMatchesAverages
                    {
                        account_id = account_id,
                        cluster = match?.cluster ?? 0,
                        match_count = 1,
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
                        win = win,
                        aghanims_scepter = player.aghanims_scepter,
                        aghanims_shard = player.aghanims_shard,
                        hero_level = player.level,
                        leaver_status = player.leaver_status,
                        moonshard = player.moonshard,
                        score = (long)score_avg,
                    };
                    playersMatchesAverages.Add(playersMatchAverage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing matchDetails for {match_seq_number}: {ex.Message}"
                );
            }

            Console.WriteLine(
                $"GetMatchDetailsAsync: Completed in {(DateTime.Now - startTime).TotalSeconds} seconds"
            );
            return new MatchDetailResponse
            {
                Match = newMatch,
                PlayersMatches = playersMatches,
                Players = players,
                playersMatchesAverages = playersMatchesAverages,
            };
        }

        public async Task<AllWithAveragesResponse?> GetAllWithAveragesAsync()
        {
            var startTime = DateTime.Now;
            var listAverages = await _dbContext
                .Set<PlayersMatchesAverages>()
                .Include(pma => pma.player)
                .GroupBy(pma => pma.cluster)
                .ToListAsync();

            if (listAverages == null || !listAverages.Any())
                return null;
            // region = RegionMapper.GetRegion(avg?.cluster ?? 0),
            var PlayersMatchesAveragesList = listAverages.Select(group => new
            {
                Cluster = group.Key,
                Players = group
                    .OrderByDescending(avg => avg.score)
                    .Select(avg => new AveragesResponse
                    {
                        score = avg.score,
                        account_id = avg.account_id,
                        assists = avg.assists ?? 0,
                        deaths = avg.deaths ?? 0,
                        denies = avg.denies ?? 0,
                        gold_per_min = avg.gold_per_min ?? 0,
                        hero_damage = avg.hero_damage ?? 0,
                        hero_healing = avg.hero_healing ?? 0,
                        kills = avg.kills ?? 0,
                        last_hits = avg.last_hits ?? 0,
                        net_worth = avg.net_worth ?? 0,
                        tower_damage = avg.tower_damage ?? 0,
                        xp_per_min = avg.xp_per_min ?? 0,
                        aghanims_scepter = avg.aghanims_scepter / avg.match_count * 100 ?? 0,
                        aghanims_shard = avg.aghanims_shard / avg.match_count * 100 ?? 0,
                        hero_level = avg.hero_level ?? 0,
                        leaver_status = avg.leaver_status / avg.match_count * 100 ?? 0,
                        moonshard = avg.moonshard / avg.match_count * 100 ?? 0,
                        matches = avg.match_count ?? 0,
                        win_rate = (double?)avg.win / avg.match_count * 100 ?? 0,
                        player = new PlayerAveragesResponse
                        {
                            avatarfull = avg?.player?.avatarfull,
                            personaname = avg?.player?.personaname,
                            loccountrycode = avg?.player?.loccountrycode,
                        },
                    }),
            });

            //PlayersMatchesAveragesList order list to score
            var PlayersMatchesAveragesOrder = PlayersMatchesAveragesList.Take(2000).ToList();

            var kills = listAverages.Average(pm => pm.kills ?? 0);
            var deaths = listAverages.Average(pm => pm.deaths ?? 0);
            var assists = listAverages.Average(pm => pm.assists ?? 0);
            var lastHits = listAverages.Average(pm => pm.last_hits ?? 0);
            var denies = listAverages.Average(pm => pm.denies ?? 0);
            var heroDamage = listAverages.Average(pm => pm.hero_damage ?? 0);
            var heroHealing = listAverages.Average(pm => pm.hero_healing ?? 0);
            var netWorth = listAverages.Average(pm => pm.net_worth ?? 0);
            var towerDamage = listAverages.Average(pm => pm.tower_damage ?? 0);
            var goldPerMin = listAverages.Average(pm => pm.gold_per_min ?? 0);
            var xpPerMin = listAverages.Average(pm => pm.xp_per_min ?? 0);
            var heroLevel = listAverages.Average(pm => pm.hero_level ?? 0);
            var wins = listAverages.Sum(pm => pm.win) ?? 0;
            var matches = listAverages.Sum(pm => pm.match_count) ?? 1;
            var winRate = (double)wins / matches * 100;
            Console.WriteLine($"winRate: {wins} / {matches} = {winRate}");
            var score = listAverages.Average(pm => pm.score);

            var averages = new AveragesAllResponse
            {
                assists = assists,
                deaths = deaths,
                denies = denies,
                kills = kills,
                last_hits = lastHits,
                hero_damage = heroDamage,
                hero_healing = heroHealing,
                net_worth = netWorth,
                tower_damage = towerDamage,
                gold_per_min = goldPerMin,
                xp_per_min = xpPerMin,
                hero_level = heroLevel,
                win_rate = winRate,
                wins = wins,
                matches = matches,
                score = score,
            };

            var result = new AllWithAveragesResponse
            {
                PlayersMatches = PlayersMatchesAveragesOrder,
                Averages = averages,
            };

            Console.WriteLine(
                $"GetAllWithAveragesAsync: Completed in {(DateTime.Now - startTime).TotalSeconds} seconds"
            );
            return result;
        }
    }
}
