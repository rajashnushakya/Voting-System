using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using VotingAPI.Model;

namespace VotingAPI.Service
{
    public class VoterService
    {
        private readonly string _connectionString;
        public VoterService(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public async Task<DbResponse> RegisterAsync(Voter voter, CancellationToken cancellationToken)
        {
            //generate user hash password
            AppUser user = new AppUser();
            user.Email = voter.Email;
            user.UserName = voter.User.UserName;
            var RoleId = 1;
            voter.User.RoleId = RoleId;
            var hasher = new PasswordHasher<AppUser>();
            var hashedPassword = hasher.HashPassword(user, voter.User.Password);

            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("sp_voter_registration");
            //voter details
            cmd.Parameters.AddWithValue("@FullName", voter.Name);
            cmd.Parameters.AddWithValue("@FatherName", voter.FatherName);
            cmd.Parameters.AddWithValue("@MotherName", voter.MotherName);
            cmd.Parameters.AddWithValue("@GrandFatherName", voter.GrandFatherName);
            cmd.Parameters.AddWithValue("@GrandMotherName", voter.GrandMotherName);
            cmd.Parameters.AddWithValue("@Email", voter.Email);
            cmd.Parameters.AddWithValue("@Gender", voter.Gender);
            cmd.Parameters.AddWithValue("@DateOfBirth", voter.DateofBirth);
            cmd.Parameters.AddWithValue("@PhoneNumber",voter.PhoneNumber);

            //voter address details
            cmd.Parameters.AddWithValue("@HouseNumber", voter.Address.HouseNumber);
            cmd.Parameters.AddWithValue("@WardId", voter.Address.WardId);
            cmd.Parameters.AddWithValue("@StreetName", voter.Address.StreetName);
            cmd.Parameters.AddWithValue("@MunicipalityId", voter.Address.MunicipalityId);
            cmd.Parameters.AddWithValue("@DistrictId", voter.Address.DistrictId);
            cmd.Parameters.AddWithValue("@Province", voter.Address.Province);

            //voter user details
            cmd.Parameters.AddWithValue("@UserName", voter.User.UserName);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@RoleId", voter.User.RoleId);

            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }


        public async Task<int> GetVoterCountAsync(CancellationToken cancellationToken)
        {
            int  voterCount= 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var cmd = new SqlCommand("sp_voter_count", connection))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        var result = await cmd.ExecuteScalarAsync(cancellationToken);
                        if (result != null)
                        {
                            voterCount = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return voterCount;
        }
        public async Task<DbResponse> VoterEnrollment(int electionId, int voterId, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            DbResponse response = null;

            SqlCommand cmd = dataAccess.CreateCommand("sp_voter_enrollment");
            cmd.Parameters.AddWithValue("@electionid", electionId);
            cmd.Parameters.AddWithValue("@voterid", voterId);
            response = await dataAccess.ExecuteNonQueryAsync(cancellationToken);

            return response;
        }

    }
}
