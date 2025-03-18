using Microsoft.Data.SqlClient;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class PartyService
    {
        private readonly string _connectionString;

        public PartyService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Party>> GetParties(CancellationToken cancellationToken)
        {
            var parties = new List<Party>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    using (var cmd = new SqlCommand("sp_get_party", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                parties.Add(new Party
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PartyId")),
                                    PartyName = reader.GetString(reader.GetOrdinal("PartyName"))
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
    }
}
