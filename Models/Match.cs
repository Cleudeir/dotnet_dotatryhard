using System.Collections.Generic;

namespace dotatryhard.Models
{
    public class Match
    {
        public long MatchId { get; set; }
        public long? StartTime { get; set; }
        public string? Cluster { get; set; }
        public short? DireScore { get; set; }
        public short? RadiantScore { get; set; }
        public short? Duration { get; set; }

        public ICollection<PlayersMatches> PlayersMatches { get; set; } = new List<PlayersMatches>();
    }
}
