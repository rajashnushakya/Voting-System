namespace VotingAPI.Model
{
    public class DbResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string SystemError { get; set; }
        public int ErrorSeverity { get; set; }
    }
}
