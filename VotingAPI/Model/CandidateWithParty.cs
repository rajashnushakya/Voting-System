namespace VotingAPI.Model
{
    public class CandidateWithParty
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
    }

}
