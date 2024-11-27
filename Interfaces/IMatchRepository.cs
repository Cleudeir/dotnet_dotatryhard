using dotatryhard.Models;

namespace dotatryhard.Interfaces
{
    public interface IMatchRepository
    {
        Task InsertMatchesAsync(List<long> matches);
    }
}
