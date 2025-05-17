 using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class DataService
    {
        private readonly string _connectionString;

        public DataService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<District>> GetAllDistrictsAsync(CancellationToken cancellationToken)
        {
            var districts = new List<District>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("GetAllDistricts", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                districts.Add(new District
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DistrictId")),
                                    Name = reader.GetString(reader.GetOrdinal("DistrictName"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching districts.", ex);
            }

            return districts;
        }


        public async Task<List<ElectionType>> GetAllElection(CancellationToken cancellationToken)
        {
            var electionType = new List<ElectionType>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("GetAllElectionTypes", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                electionType.Add(new ElectionType
                                {
                                    ElectionTypeId = reader.GetInt32(reader.GetOrdinal("ElectionTypeId")),
                                    ElectionTypeName = reader.GetString(reader.GetOrdinal("ElectionTypeName"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching election type.", ex);
            }

            return electionType;
        }


        public async Task<List<CandidatesPosition>> GetCandidatePosition(CancellationToken cancellationToken)
        {
            var CandidatePosition = new List<CandidatesPosition>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("GetAllCandidatePositions", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                CandidatePosition.Add(new CandidatesPosition
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    PositionName = reader.GetString(reader.GetOrdinal("PositionName"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching Candidate Position.", ex);
            }

            return CandidatePosition;
        }

        public async Task<List<Municipality>> GetMunicipalitiesAsync(int? districtId, CancellationToken cancellationToken)
        {
            var municipalities = new List<Municipality>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("GetMunicipalitiesByDistrict", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        if (districtId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@DistrictId", districtId.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DistrictId", DBNull.Value);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                municipalities.Add(new Municipality
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("MunicipalityId")),
                                    Name = reader.GetString(reader.GetOrdinal("MunicipalityName")),
                                    DistrictId = reader.GetInt32(reader.GetOrdinal("DistrictId")),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching municipalities.", ex);
            }

            return municipalities;
        }
        public async Task<List<Ward>> GetWardAsync(int? municipalityId, CancellationToken cancellationToken)
        {
            var wards = new List<Ward>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("GetWardsByMunicipality", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        if (municipalityId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@municipality_id", municipalityId.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@municipality_id", DBNull.Value);
                        }

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                wards.Add(new Ward
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")), 
                                    municipalityId = reader.GetInt32(reader.GetOrdinal("municipality_id")), 
                                    WardNumber = reader.GetInt32(reader.GetOrdinal("ward_number")) 
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching wards.", ex);
            }

            return wards;
        }
        public async Task<List<CentreDetails>> GetCentreDetailsAsync(int electionId, CancellationToken cancellationToken)
        {
            var centreDetailsList = new List<CentreDetails>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("GetCentreDetailsByElectionId", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@electionId", electionId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                centreDetailsList.Add(new CentreDetails
                                {
                                    CentreId = reader.GetInt32(reader.GetOrdinal("CentreId")),
                                    CentreName = reader.GetString(reader.GetOrdinal("CentreName")),
                                    DistrictName = reader.GetString(reader.GetOrdinal("DistrictName")),
                                    MunicipalityName = reader.GetString(reader.GetOrdinal("MunicipalityName")),
                                    WardNumber = reader.GetInt32(reader.GetOrdinal("WardNumber"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching centre details by election ID.", ex);
            }

            return centreDetailsList;
        }
        public async Task<List<CandidateVotesByCentre>> GetCandidateVotesByCentreAsync(int CentreId, CancellationToken cancellationToken)
        {
            var candidateVotesList = new List<CandidateVotesByCentre>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    // Fetch the candidate votes by centre
                    using (var cmd = new SqlCommand("GetCandidateVotesByCentre", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CentreId", CentreId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var candidateVotes = new CandidateVotesByCentre
                                {
                                    CandidateName = reader.GetString(reader.GetOrdinal("CandidateName")),
                                    PartyName = reader.GetString(reader.GetOrdinal("PartyName")),
                                    Votes = reader.GetInt32(reader.GetOrdinal("Votes")),
                                    VotePercentage = reader.GetDecimal(reader.GetOrdinal("VotePercentage"))
                                };

                                candidateVotesList.Add(candidateVotes);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching candidate votes by centre.", ex);
            }

            return candidateVotesList;
        }

        public async Task<List<ElectionCentreDetails>> GetElectionCentreDetailsAsync(int CentreId, CancellationToken cancellationToken)
        {
            var centreDetailsList = new List<ElectionCentreDetails>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    // Fetch the centre details along with the counts (candidates, voters, votes) using a single SP
                    using (var cmd = new SqlCommand("GetCandidatesAndVotersCountByCentreId", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@centreId", CentreId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var centreDetails = new ElectionCentreDetails
                                {
                                    TotalCandidates = reader.GetInt32(reader.GetOrdinal("TotalCandidates")),
                                    TotalVoters = reader.GetInt32(reader.GetOrdinal("TotalVoters")),
                                    TotalVotes = reader.GetInt32(reader.GetOrdinal("TotalVotes"))
                                };

                                centreDetailsList.Add(centreDetails);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching election centre details.", ex);
            }

            return centreDetailsList;
        }



        public async Task<List<Party>> GetAllPartiesAsync()
        {
            var parties = new List<Party>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var cmd = new SqlCommand("GetAllParties", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                parties.Add(new Party
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PartyId")),
                                    PartyName = reader.GetString(reader.GetOrdinal("PartyName")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Logo = reader.GetString(reader.GetOrdinal("Logo"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching parties.", ex);
            }

            return parties;
        }

        public async Task<DbResponse> AddCentreAsync(Centre centre, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("sp_add_centre");

            cmd.Parameters.AddWithValue("@CentreName", centre.name);
            cmd.Parameters.AddWithValue("@district_id", centre.districtId);
            cmd.Parameters.AddWithValue("@municipality_id", centre.municipalityId);
            cmd.Parameters.AddWithValue("@ward_id", centre.wardId);

            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }
        public async Task<List<Municipality>> GetElectionMunicipality(int districtId, CancellationToken cancellationToken)
        {
            var municipalities = new List<Municipality>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_getmunicipality", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@district_id", districtId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                municipalities.Add(new Municipality
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("municipality_id")),
                                    Name = reader.GetString(reader.GetOrdinal("municipality_name"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching municipalities by district.", ex);
            }

            return municipalities;
        }
        public async Task<List<Ward>> GetElectionWard(int municipalityId, CancellationToken cancellationToken)
        {
            var ward = new List<Ward>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_getWard", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@municipality_id", municipalityId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                ward.Add(new Ward
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ward_id")),
                                    WardNumber = reader.GetInt32(reader.GetOrdinal("ward_name"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching ward by municipality.", ex);
            }

            return ward;
        }

        public async Task<List<Centre>> GetCentreName(int districtId, int municipalityId, int wardId, CancellationToken cancellationToken)
        {
            var name = new List<Centre>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_get_centre_name", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@district_id", districtId);
                        cmd.Parameters.AddWithValue("@municipality_id", municipalityId);
                        cmd.Parameters.AddWithValue("@ward_id", wardId);

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                name.Add(new Centre
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    name = reader.GetString(reader.GetOrdinal("name")) 
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the centre name.", ex);
            }

            return name;
        }


    }

}

