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

            //voter address details
            cmd.Parameters.AddWithValue("@HouseNumber", voter.Address.HouseNumber);
            cmd.Parameters.AddWithValue("@WardNumber", voter.Address.WardNumber);
            cmd.Parameters.AddWithValue("@StreetName", voter.Address.StreetName);
            cmd.Parameters.AddWithValue("@Municipality", voter.Address.Municpality);
            cmd.Parameters.AddWithValue("@District", voter.Address.District);
            cmd.Parameters.AddWithValue("@Province", voter.Address.Province);

            //voter user details
            cmd.Parameters.AddWithValue("@UserName", voter.User.UserName);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@RoleId", voter.User.RoleId);

            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
