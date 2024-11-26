using System.Collections.Generic;

namespace dotatryhard.Models
{
    public class Player
    {
        public long AccountId { get; set; }
        public string? PersonaName { get; set; }
        public string? AvatarFull { get; set; }
        public string? LocCountryCode { get; set; }

        public ICollection<PlayersMatches> PlayersMatches { get; set; } = new List<PlayersMatches>();
    }
}
