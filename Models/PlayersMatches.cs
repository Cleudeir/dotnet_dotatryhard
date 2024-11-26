namespace dotatryhard.Models
{
    public class PlayersMatches
    {
        public long account_id { get; set; }
        public long match_id { get; set; }
        public short? assists { get; set; }
        public short? deaths { get; set; }
        public short? kills { get; set; }
        public short? gold_per_min { get; set; }
        public short? xp_per_min { get; set; }

        public required Player player { get; set; }
        public required Match match { get; set; }
    }
}
