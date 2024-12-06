namespace dotatryhard.Interfaces
{
    public class DotaMatchHistoryResponse
    {
        public required ResultMatchHistory result { get; set; }
    }

    public class ResultMatchHistory
    {
        public int status { get; set; }
        public int num_results { get; set; }
        public int total_results { get; set; }
        public int results_remaining { get; set; }
        public required List<MatchMatchHistory> matches { get; set; }
    }

    public class MatchMatchHistory
    {
        public long match_id { get; set; }
        public long match_seq_num { get; set; }
        public long start_time { get; set; }
        public int lobby_type { get; set; }
        public int radiant_team_id { get; set; }
        public int dire_team_id { get; set; }
        public required List<PlayerMatchHistory> players { get; set; }
    }

    public class PlayerMatchHistory
    {
        public long account_id { get; set; }
        public int player_slot { get; set; }
        public int team_number { get; set; }
        public int team_slot { get; set; }
        public int hero_id { get; set; }
        public int hero_variant { get; set; }
    }
}
