using dotatryhard.Models;

namespace dotatryhard.Interfaces
{
    public interface IPlayerRepository
    {
        Task InsertPlayersAsync(List<int> players);
    }
}
