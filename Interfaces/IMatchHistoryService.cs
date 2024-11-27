namespace dotatryhard.Interfaces
{
    public interface IMatchHistoryService
    {
        Task<(List<long> Matches, List<int> Players)?> GetMatchHistoryAsync(int accountId);
    }
}
