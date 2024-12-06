namespace dotatryhard.Interfaces
{
    public class DotaGetMatchHistoryBySequenceNumResponse
    {
        public required ResultMatchHistoryBySequence result { get; set; }
    }


    public class ResultMatchHistoryBySequence
    {
        public int status { get; set; }
        public required List<MatchMatchHistoryBySequence> matches { get; set; }
    }

    public class AbilityUpgradeMatchHistoryBySequence
    {
        public int ability { get; set; }
        public int time { get; set; }
        public int level { get; set; }
    }

    public class MatchMatchHistoryBySequence
    {
        public required List<PlayerMatchHistoryBySequence> players { get; set; }
        public bool radiant_win { get; set; }
        public short duration { get; set; }
        public int pre_game_duration { get; set; }
        public int start_time { get; set; }
        public long match_id { get; set; }
        public long match_seq_num { get; set; }
        public int tower_status_radiant { get; set; }
        public int tower_status_dire { get; set; }
        public int barracks_status_radiant { get; set; }
        public int barracks_status_dire { get; set; }
        public int cluster { get; set; }
        public int first_blood_time { get; set; }
        public int lobby_type { get; set; }
        public int human_players { get; set; }
        public int leagueid { get; set; }
        public int game_mode { get; set; }
        public int flags { get; set; }
        public int engine { get; set; }
        public short radiant_score { get; set; }
        public short dire_score { get; set; }
    }

    public class PlayerMatchHistoryBySequence
    {
        public long account_id { get; set; }
        public short player_slot { get; set; }
        public byte team_number { get; set; }
        public int team_slot { get; set; }
        public short hero_id { get; set; }
        public int hero_variant { get; set; }
        public short item_0 { get; set; }
        public short item_1 { get; set; }
        public short item_2 { get; set; }
        public short item_3 { get; set; }
        public short item_4 { get; set; }
        public short item_5 { get; set; }
        public short backpack_0 { get; set; }
        public short backpack_1 { get; set; }
        public short backpack_2 { get; set; }
        public short item_neutral { get; set; }
        public short kills { get; set; }
        public short deaths { get; set; }
        public short assists { get; set; }
        public byte leaver_status { get; set; }
        public short last_hits { get; set; }
        public short denies { get; set; }
        public short gold_per_min { get; set; }
        public short xp_per_min { get; set; }
        public short level { get; set; }
        public int net_worth { get; set; }
        public byte aghanims_scepter { get; set; }
        public byte aghanims_shard { get; set; }
        public byte moonshard { get; set; }
        public int hero_damage { get; set; }
        public int tower_damage { get; set; }
        public int hero_healing { get; set; }
        public int gold { get; set; }
        public int gold_spent { get; set; }
        public int scaled_hero_damage { get; set; }
        public int scaled_tower_damage { get; set; }
        public int scaled_hero_healing { get; set; }
        public required List<AbilityUpgradeMatchHistoryBySequence> ability_upgrades { get; set; }
    }

}





