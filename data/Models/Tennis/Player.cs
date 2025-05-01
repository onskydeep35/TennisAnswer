namespace Data.Models.Tennis
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hand { get; set; }
        public DateTime? Dob { get; set; }
        public string Ioc { get; set; }
        public string Country { get; set; }
        public int? Height { get; set; }
        public string WikiDataId { get; set; }
    }
}
