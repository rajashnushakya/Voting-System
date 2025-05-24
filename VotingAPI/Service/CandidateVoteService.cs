using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using VotingAPI.Model;

public class CandidateVoteService
{
    private readonly string _connectionString;

    public CandidateVoteService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<int> InsertCandidateVoteAsync(int candidateId, int voterId, int electionId, int electionCentreId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand("InsertCandidateVote", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameters for the stored procedure
            cmd.Parameters.AddWithValue("@CandidateId", candidateId);
            cmd.Parameters.AddWithValue("@VoterId", voterId);
            cmd.Parameters.AddWithValue("@ElectionId", electionId);
            cmd.Parameters.AddWithValue("@ElectionCentreId", electionCentreId);

            // Open connection and execute the command
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
    public async Task<List<VoteDetails>> GetVotesByVoterIdAsync(int voterId)
    {
        var voteDetailsList = new List<VoteDetails>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand("GetVotesByVoterId", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VoterId", voterId);

            // Open connection
            await conn.OpenAsync();

            // Execute the command and read the results
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var voteDetail = new VoteDetails
                    {
                        VoterId = reader.GetInt32(reader.GetOrdinal("VoterId")),
                        CandidateId = reader.GetInt32(reader.GetOrdinal("CandidateId")),
                        CandidateName = reader.GetString(reader.GetOrdinal("CandidateName")),
                        PartyId = reader.GetInt32(reader.GetOrdinal("PartyId")),
                        PartyName = reader.GetString(reader.GetOrdinal("PartyName")),
                        ElectionId = reader.GetInt32(reader.GetOrdinal("ElectionId")),
                        ElectionName = reader.GetString(reader.GetOrdinal("ElectionName")),
                        ElectionCentreId = reader.GetInt32(reader.GetOrdinal("ElectionCentreId")),
                        CentreName = reader.GetString(reader.GetOrdinal("CentreName"))
                    };

                    voteDetailsList.Add(voteDetail);
                }
            }
        }

        return voteDetailsList;
    }
}
