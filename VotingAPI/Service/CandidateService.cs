using Microsoft.Data.SqlClient;
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
    }
}
