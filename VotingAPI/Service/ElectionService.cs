using Microsoft.Data.SqlClient;
using VotingAPI.Model;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace VotingAPI.Service
{
    public class ElectionService
    {
        private readonly string _connectionString;

        public ElectionService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        // Method to add a new election
        public async Task<DbResponse> AddElectionAsync(Election election, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("sp_election_add");

            // Adding parameters to the command
            cmd.Parameters.AddWithValue("@ElectionName", election.Name);
            cmd.Parameters.AddWithValue("@StartDate", election.Start_date);
            cmd.Parameters.AddWithValue("@EndDate", election.End_date);

            // Execute the command (non-query)
            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }
        public async Task<int> GetElectionCountAsync(CancellationToken cancellationToken)
        {
            int electionCount = 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("GetElectionCount", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        var result = await cmd.ExecuteScalarAsync(cancellationToken);
                        if (result != null)
                        {
                            electionCount = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw; 
            }

            return electionCount;
        }

        public async Task<List<Election>> GetActiveElectionAsync(CancellationToken cancellationToken)
        {
            var elections = new List<Election>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_get_active_elections", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                elections.Add(new Election
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),  
                                    Start_date = reader.GetDateTime(reader.GetOrdinal("Start_date")),  
                                    End_date = reader.GetDateTime(reader.GetOrdinal("End_date"))  
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return elections;
        }


    }
}
