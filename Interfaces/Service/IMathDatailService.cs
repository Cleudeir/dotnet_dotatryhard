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
    }
    public class AllWithAveragesResponse {
        public required List<PlayersMatches> PlayersMatches { get; set; }
        public required IDictionary<string, double> Averages { get; set; }
    }
}
