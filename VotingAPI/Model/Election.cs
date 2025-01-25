using Microsoft.VisualBasic;

namespace VotingAPI.Model
{
    public class Election
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly Start_date { get; set; }
        public DateOnly End_date { get; set; }
    }
}
