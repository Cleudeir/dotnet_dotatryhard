namespace dotatryhard.Models
{
    public class PlayersMatches
    {
        public long account_id { get; set; }
        public long match_id { get; set; }
        public short? last_hits { get; set; }
        public short? denies { get; set; }
        public short? assists { get; set; }
        public short? deaths { get; set; }
        public short? kills { get; set; }
        public int? hero_damage { get; set; }
        public int? hero_healing { get; set; }
        public int? net_worth { get; set; }
        public int? tower_damage { get; set; }
        public short? gold_per_min { get; set; }
        public short? xp_per_min { get; set; }
        public string? ability_0 { get; set; }
        public string? ability_1 { get; set; }
        public string? ability_2 { get; set; }
        public string? ability_3 { get; set; }
        public short? hero_level { get; set; }
        public byte? team { get; set; }
        public byte? leaver_status { get; set; }
        public short? aghanims_scepter { get; set; }
        public short? aghanims_shard { get; set; }
        public short? backpack_0 { get; set; }
        public short? backpack_1 { get; set; }
        public short? backpack_2 { get; set; }
        public string? item_0 { get; set; }
        public string? item_1 { get; set; }
        public string? item_2 { get; set; }
        public string? item_3 { get; set; }
        public string? item_4 { get; set; }
        public string? item_5 { get; set; }
        public short? item_neutral { get; set; }
        public short? moonshard { get; set; }
        public string? hero_id { get; set; }
        public short? player_slot { get; set; }
         public bool win { get; set; }
        public int? score { get; set; }

        public required Player player { get; set; }
        public required Match match { get; set; }
    }
}
