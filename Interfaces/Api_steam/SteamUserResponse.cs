namespace dotatryhard.Interfaces
{
    public class PlayerSteamUserResponse
    {
        public required string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string? personaname { get; set; }
        public int commentpermission { get; set; }
        public string? profileurl { get; set; }
        public string? avatar { get; set; }
        public string? avatarmedium { get; set; }
        public string? avatarfull { get; set; }
        public string? avatarhash { get; set; }
        public int lastlogoff { get; set; }
        public int personastate { get; set; }
        public string? primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
        public string? gameextrainfo { get; set; }
        public string? gameid { get; set; }
        public string? loccountrycode { get; set; }
    }

    public class ResponseSteamUserResponse
    {
        public required List<PlayerSteamUserResponse> players { get; set; }
    }

    public class SteamUserResponse
    {
        public ResponseSteamUserResponse? response { get; set; }
    }


}
