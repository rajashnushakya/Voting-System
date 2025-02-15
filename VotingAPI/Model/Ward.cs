namespace VotingAPI.Model
{
    public class Ward
    {
        public int Id { get; set; }
        public int WardNumber { get; set; }
        public int municipalityId { get; set; }
        public Municipality Municipality { get; set; }

    }
}
