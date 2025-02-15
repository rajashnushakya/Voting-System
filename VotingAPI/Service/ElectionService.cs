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
        public async Task<List<Election>> GetElectionAsync(Election election, CancellationToken cancellationToken)
        {
            var elections = new List<Election>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_get_election", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ElectionName", election.Name);
                        cmd.Parameters.AddWithValue("@StartDate", election.Start_date);
                        cmd.Parameters.AddWithValue("@EndDate", election.End_date );

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                elections.Add(new Election
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("ElectionName")),
                                    Start_date = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    End_date = reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching elections.", ex);
            }

            return elections;
        }

    }
}
