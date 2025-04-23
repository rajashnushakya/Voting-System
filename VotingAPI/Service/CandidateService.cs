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
            cmd.Parameters.AddWithValue("@Position", candidate.Position);
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
                                    
                                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                    PartyId = reader.GetInt32(reader.GetOrdinal("PartyId")),
                                    FatherName = reader.GetString(reader.GetOrdinal("FatherName")),
                                    MotherName = reader.GetString(reader.GetOrdinal("MotherName")),
                                    GrandFatherName = reader.GetString(reader.GetOrdinal("GrandFatherName")),
                                    GrandMotherName = reader.GetString(reader.GetOrdinal("GrandMotherName")),
                                    Dateofbirth = reader.GetFieldValue<DateOnly>(reader.GetOrdinal("Dateofbirth")),
                                    gender = reader.GetString(reader.GetOrdinal("gender")),
                                    ElectionId = reader.GetInt32(reader.GetOrdinal("ElectionId")),
                                    DistrictId = reader.GetInt32(reader.GetOrdinal("DistrictId")),
                                    MunicipalityId = reader.GetInt32(reader.GetOrdinal("MunicipalityId")),
                                    WardId = reader.GetInt32(reader.GetOrdinal("WardId"))

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

        public async Task<DbResponse>centreCandidateAsync(List<CandidateCentre> CandidateCentre, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            DbResponse response = null;

            foreach (var candidate in CandidateCentre)
            {
                SqlCommand cmd = dataAccess.CreateCommand("InsertCandidateElectionCentre");

                cmd.Parameters.AddWithValue("@CandidateId", candidate.CandidateId);

                cmd.Parameters.AddWithValue("@CentreId", candidate.CentreId);
                response = await dataAccess.ExecuteNonQueryAsync(cancellationToken);

            }
            return response;
        }

        public async Task<List<Candidate>> GetCandidatesByElectionIdAsync(int electionId, CancellationToken cancellationToken)
        {
            var candidates = new List<Candidate>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("GetCandidatesByElectionId", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ElectionId", electionId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            
                            while (await reader.ReadAsync())
                            {
                                candidates.Add(new Candidate
                                {

                                    Id = reader.GetInt32(reader.GetOrdinal("CandidateId")).ToString(),


                                    FullName = reader.GetString(reader.GetOrdinal("FullName"))
                                });
                            }
                            //if (await reader.NextResultAsync(cancellationToken))
                            //{
                            //    while (await reader.ReadAsync(cancellationToken))
                            //    {
                            //        int status = reader.GetInt32(reader.GetOrdinal("status"));
                            //        string message = reader.GetString(reader.GetOrdinal("error_msg"));
                            //        string sysError = reader.GetString(reader.GetOrdinal("sys_error"));
                            //        int severity = reader.GetInt32(reader.GetOrdinal("error_severity"));

                                  
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching candidates by election ID.", ex);
            }

            return candidates;
        }

        public async Task<List<CandidateWithParty>> GetCandidatesByCentreIdAsync(int centreId, CancellationToken cancellationToken)
        {
            var candidates = new List<CandidateWithParty>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("GetCandidatesByCentreId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CentreId", centreId);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync())
                    {
                        candidates.Add(new CandidateWithParty
                        {
                            CandidateId = reader.GetInt32(reader.GetOrdinal("candidateid")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            PartyId = reader.GetInt32(reader.GetOrdinal("partyid")),
                            PartyName = reader.GetString(reader.GetOrdinal("partyname"))
                        });
                    }
                }
            }

            return candidates;
        }
    }
}
