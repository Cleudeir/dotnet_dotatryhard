namespace dotatryhard.Models
{
    public class PlayersMatches
    {
        public long AccountId { get; set; }
        public long MatchId { get; set; }
        public short? Assists { get; set; }
        public short? Deaths { get; set; }
        public short? Kills { get; set; }
        public short? GoldPerMin { get; set; }
        public short? XpPerMin { get; set; }

        public Player Player { get; set; }
        public Match Match { get; set; }
    }
}
