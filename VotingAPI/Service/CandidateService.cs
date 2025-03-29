using Microsoft.Data.SqlClient;
using System.Data;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class CandidateService
    {
        private readonly string _connectionString;

        public CandidateService(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public async Task<DbResponse> AddCandidateAsync(Candidate candidate, CancellationToken cancellationToken)
        {
           
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("InsertCandidate"); 
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FullName", candidate.FullName);
            cmd.Parameters.AddWithValue("@PartyId", candidate.PartyId);
            cmd.Parameters.AddWithValue("@FatherName", candidate.FatherName);
            cmd.Parameters.AddWithValue("@MotherName", candidate.MotherName);
            cmd.Parameters.AddWithValue("@GrandFatherName", candidate.GrandFatherName);
            cmd.Parameters.AddWithValue("@GrandMotherName", candidate.GrandMotherName);
            cmd.Parameters.AddWithValue("@DateOfBirth", candidate.Dateofbirth);
            cmd.Parameters.AddWithValue("@Gender", candidate.gender);
            cmd.Parameters.AddWithValue("@ElectionId", candidate.ElectionId);
            cmd.Parameters.AddWithValue("@DistrictId", candidate.DistrictId);
            cmd.Parameters.AddWithValue("@MunicipalityId", candidate.MunicipalityId);
            cmd.Parameters.AddWithValue("@WardId", candidate.WardId);
            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
            
        }
        public async Task<List<Candidate>> GetCandidatesAsync(int voterId, CancellationToken cancellationToken)
        {
            var candidates = new List<Candidate>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("sp_get_candidates_by_voter_address", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@voterId", voterId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                candidates.Add(new Candidate
                                {
                                    
                                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching candidates.", ex);
            }

            return candidates;
        }

    }
}
