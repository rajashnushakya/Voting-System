namespace VotingAPI.Model
{
    public class CandidateVotesByCentre
    {
        public string CandidateName { get; set; } // Candidate's Full Name
        public string PartyName { get; set; } // Party Name
        public int Votes { get; set; } // Total Votes for Candidate
        public decimal VotePercentage { get; set; } // Vote Percentage (rounded to 2 decimal places)
    }


}
