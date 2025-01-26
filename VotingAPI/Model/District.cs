namespace VotingAPI.Model
{
    public class District
    {
        public int Id { get; set; } 
        public string Name { get; set; }

        public List<Municipality> Municipalities { get; set; }
    }

}
