namespace dotatryhard.Models
{
    public class Match
    {
        public long match_id { get; set; }
        public long? match_seq_num { get; set; }
        public long? start_time { get; set; }
        public string? cluster { get; set; }
        public short? dire_score { get; set; }
        public short? radiant_score { get; set; }
        public short? duration { get; set; }

        public ICollection<PlayersMatches> PlayersMatches { get; set; } =
            new List<PlayersMatches>();
    }
}
