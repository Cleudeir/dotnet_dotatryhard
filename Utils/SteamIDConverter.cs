namespace dotatryhard.Utils{
class SteamIDConverter
{
    public static long ConvertAccountIDToSteamID64(long accountID)
    {
        // The base value for SteamID64 (constant offset)
        const long SteamID64Base = 76561197960265728;

        // Convert AccountID to SteamID64
        return SteamID64Base + accountID;
    }

}
}

