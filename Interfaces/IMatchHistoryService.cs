namespace dotatryhard.Interfaces
{
    public interface IMatchHistoryService
    {
        Task<(List<MatchInfo> Matches, List<int> Players)?> GetMatchHistoryAsync(int accountId);
    }
    public class MatchInfo
{
    public long id { get; set; }
    public long seq_num { get; set; }
}
}
