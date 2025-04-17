namespace VotingAPI.Model
{
    public class Vote
    {
        public int CandidateId { get; set; }
        public int VoterId { get; set; }
        public int ElectionId { get; set; }
        public int ElectionCentreId { get; set; }
    }
}
