using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Tennis
{
    [Table("tennis_players")]
    public class Player
    {
        [Column("player_id")]
        public int PlayerId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("hand")]
        public string Hand { get; set; }

        [Column("date_of_birth")]
        public DateTime? Dob { get; set; }

        [Column("ioc")]
        public string Ioc { get; set; }

        [Column("country")]
        public string Country { get; set; }

        [Column("height")]
        public int? Height { get; set; }

        [Column("wikidata_id")]
        public string WikiDataId { get; set; }
    }
}
