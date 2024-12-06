namespace dotatryhard.Interfaces
{
    public interface IMatchHistoryService
    {
        Task<(List<MatchInfo> Matches, List<long> Players)?> GetMatchHistoryAsync(long accountId);
    }
    public class MatchInfo
{
    public long id { get; set; }
    public long seq_num { get; set; }
}
}
