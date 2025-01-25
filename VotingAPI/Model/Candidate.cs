using Microsoft.OpenApi.Writers;

namespace VotingAPI.Model
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        public string FullName { get; set; }

        public int PartyId { get; set; }
        public string FatherName { get; set; }

        public string MotherName { get; set; }
        public string GrandFatherName { get; set; }
        public string GrandMotherName { get; set; }
        public DateOnly Dateofbirth { get; set; }
        public string gender { get; set; }
        public string  District {  get; set; }
        public string Municipality { get; set; }



    }
}
