namespace VotingAPI.Model
{
    public class CandidateVoteResult
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }
        public int VoteCount { get; set; }
        public string ElectionName { get; set; }
    }

}
