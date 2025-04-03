using Microsoft.Data.SqlClient;
using VotingAPI.Model;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Azure;

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

        public async Task<DbResponse> AddElectionCentreAsync(List<ElectionCentre> electionCentre, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            DbResponse response = null;

            foreach (var centre in electionCentre)
            {
                SqlCommand cmd = dataAccess.CreateCommand("sp_add_election_centre");

                cmd.Parameters.AddWithValue("@CentreName", centre.electionCentre); 

                cmd.Parameters.AddWithValue("@ElectionId", centre.ElectionId);
                response = await dataAccess.ExecuteNonQueryAsync(cancellationToken);

            }
            return response;
        }

        public async Task<DbResponse> EndElectionAsync(int electionId, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            DbResponse response = null;

            SqlCommand cmd = dataAccess.CreateCommand("EndElection");
            cmd.Parameters.AddWithValue("@ElectionId", electionId);
            response = await dataAccess.ExecuteNonQueryAsync(cancellationToken);

            return response;
        }

        public async Task<List<Centre>> GetElectionCentre(int? electionId, CancellationToken cancellationToken)
        {
            var centres = new List<Centre>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("GetCentresByElectionId", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        if (electionId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ElectionId", electionId.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ElectionId", DBNull.Value);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                centres.Add(new Centre
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.GetString(reader.GetOrdinal("name")),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching centre.", ex);
            }

            return centres;
        }

        public async Task<DbResponse> InsertElectionCentreVoterAsync(int centreId, int voterId, CancellationToken cancellationToken)
        {
            DbResponse response = new DbResponse();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("InsertElectionCentreVoter", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@CentreId", centreId);
                        cmd.Parameters.AddWithValue("@VoterId", voterId);

                        // Execute the stored procedure
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error", ex);
            }

            return response;
        }


    }
}
