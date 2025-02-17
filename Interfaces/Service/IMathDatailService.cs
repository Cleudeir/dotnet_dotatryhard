using dotatryhard.Models;

namespace dotatryhard.Interfaces
{
    public interface IMatchDetailService
    {
        Task<MatchDetailResponse?> GetMatchDetailsAsync(long accountId);
        Task<AllWithAveragesResponse?> GetAllWithAveragesAsync();
    }

    public class MatchDetailResponse
    {
        public required Match Match { get; set; }
        public required List<PlayersMatches> PlayersMatches { get; set; }
        public required HashSet<long>? Players { get; set; }
        public required List<PlayersMatchesAverages> playersMatchesAverages { get; set; }
    }

    public class PlayerAveragesResponse
    {
        public string? personaname { get; set; }
        public string? avatarfull { get; set; }
        public string? loccountrycode { get; set; }
    }

    public class AveragesResponse
    {
        public long account_id { get; set; }
        public int position { get; set; }
        public int matches { get; set; }
        public int last_hits { get; set; }
        public int denies { get; set; }
        public int assists { get; set; }
        public int deaths { get; set; }
        public int kills { get; set; }
        public int hero_damage { get; set; }
        public int hero_healing { get; set; }
        public int net_worth { get; set; }
        public int tower_damage { get; set; }
        public int gold_per_min { get; set; }
        public int xp_per_min { get; set; }
        public int hero_level { get; set; }
        public int leaver_status { get; set; }
        public int aghanims_scepter { get; set; }
        public int aghanims_shard { get; set; }
        public int moonshard { get; set; }
        public double win_rate { get; set; }
        public long score { get; set; }
        public PlayerAveragesResponse? player { get; set; }
    }

    public class AveragesAllResponse
    {
        public double assists { get; set; }
        public double deaths { get; set; }
        public double denies { get; set; }
        public double kills { get; set; }
        public double last_hits { get; set; }
        public double hero_damage { get; set; }
        public double hero_healing { get; set; }
        public double net_worth { get; set; }
        public double tower_damage { get; set; }
        public double gold_per_min { get; set; }
        public double xp_per_min { get; set; }
        public double hero_level { get; set; }
        public double win_rate { get; set; }
        public int wins { get; set; }
        public int matches { get; set; }
        public double score { get; set; }
    }

    public class AllWithAveragesResponse
    {
        public required List<ClusterPlayersResponse> playersMatches { get; set; }
        public required List<ClusterAveragesResponse> averages { get; set; }
    }

    public class ClusterPlayersResponse
    {
        public string? region { get; set; }
        public required IEnumerable<AveragesResponse> data { get; set; }
    }

    public class ClusterAveragesResponse
    {
        public string? region { get; set; }
        public required AveragesAllResponse data { get; set; }
    }

    public class AllMatchesPlayerResponse
    {
        public required List<PlayersMatches> playersMatches { get; set; }
    }
}
