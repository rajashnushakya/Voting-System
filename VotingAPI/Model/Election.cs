using Microsoft.VisualBasic;

namespace VotingAPI.Model
{
    public class Election
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }

        public int type_id { get; set; }
    }
}
