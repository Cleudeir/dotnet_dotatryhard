namespace dotatryhard.Models
{
    public class Player
    {
        public long account_id { get; set; }
        public string? personaname { get; set; }
        public string? avatarfull { get; set; }
        public string? loccountrycode { get; set; }

        public ICollection<PlayersMatches> PlayersMatches { get; set; } = new List<PlayersMatches>();
    }
}
