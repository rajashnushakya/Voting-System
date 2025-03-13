 using System;
using System.Collections.Generic;
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

    }

}

