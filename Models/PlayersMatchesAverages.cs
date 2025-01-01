namespace dotatryhard.Models
{
    public class PlayersMatchesAverages
    {
        public int? cluster { get; set; }
        public long account_id { get; set; }
        public int? match_count { get; set; }
        public int? last_hits { get; set; }
        public int? denies { get; set; }
        public int? assists { get; set; }
        public int? deaths { get; set; }
        public int? kills { get; set; }
        public int? hero_damage { get; set; }
        public int? hero_healing { get; set; }
        public int? net_worth { get; set; }
        public int? tower_damage { get; set; }
        public int? gold_per_min { get; set; }
        public int? xp_per_min { get; set; }
        public int? hero_level { get; set; }
        public int? leaver_status { get; set; }
        public int? aghanims_scepter { get; set; }
        public int? aghanims_shard { get; set; }
        public int? moonshard { get; set; }
        public int? win { get; set; }
        public long score { get; set; }
        public Player? player { get; set; }
    }
}
