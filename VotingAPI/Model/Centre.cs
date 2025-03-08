namespace VotingAPI.Model
{
    public class Centre
    {
        public int id { get; set; }
        public string name { get; set; }
        public int districtId { get; set; }
        public int municipalityId { get; set; }
        public int wardId { get; set; }
    }
}
