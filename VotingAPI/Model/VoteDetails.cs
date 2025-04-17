namespace VotingAPI.Model
{
    public class VoteDetails
    {
        public int VoterId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int ElectionId { get; set; }
        public string ElectionName { get; set; }
        public int ElectionCentreId { get; set; }
        public string ElectionCentreName { get; set; }
    }
}
