using dotatryhard.Models;

namespace dotatryhard.Interfaces
{
    public interface ISteamUserService
    {
        Task<Player?> FetchProfilesAsync(long accountId);
    }
}


