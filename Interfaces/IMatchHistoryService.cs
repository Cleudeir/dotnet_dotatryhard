namespace dotatryhard.Interfaces
{
    public interface IMatchHistoryService
    {
        Task<(List<int> Matches, List<int> Players)?> GetMatchHistoryAsync(int accountId);
    }
}
