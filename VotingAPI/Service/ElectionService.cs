using Microsoft.Data.SqlClient;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class ElectionService
    {

        private readonly string _connectionString;
        public ElectionService(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public async Task<DbResponse> AddElectionAsync(Election election, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("sp_election_add");
            //voter details
            cmd.Parameters.AddWithValue("@ElectionName", election.Name);
            cmd.Parameters.AddWithValue("@StartDate", election.Start_date);
            cmd.Parameters.AddWithValue("@EndDate", election.Name);
            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
