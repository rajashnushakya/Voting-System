﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            int age = DateTime.Today.Year - voter.DateofBirth.Year;
            if (voter.DateofBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                return new DbResponse
                {
                    Status = 100,
                    Message = "Voter must be at least 18 years old."
                };
            }

            // Hash the password
            voter.User.RoleId = 1;
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(voter.User.Password, 5);

            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("sp_voter_registration");

            // voter details
            cmd.Parameters.AddWithValue("@FullName", voter.Name);
            cmd.Parameters.AddWithValue("@FatherName", voter.FatherName);
            cmd.Parameters.AddWithValue("@MotherName", voter.MotherName);
            cmd.Parameters.AddWithValue("@GrandFatherName", voter.GrandFatherName);
            cmd.Parameters.AddWithValue("@GrandMotherName", voter.GrandMotherName);
            cmd.Parameters.AddWithValue("@Email", voter.Email);
            cmd.Parameters.AddWithValue("@Gender", voter.Gender);
            cmd.Parameters.AddWithValue("@DateOfBirth", voter.DateofBirth);
            cmd.Parameters.AddWithValue("@PhoneNumber", voter.PhoneNumber);
            cmd.Parameters.AddWithValue("@NationalId", voter.NationalId);

            // address details
            cmd.Parameters.AddWithValue("@HouseNumber", voter.Address.HouseNumber);
            cmd.Parameters.AddWithValue("@WardId", voter.Address.WardId);
            cmd.Parameters.AddWithValue("@StreetName", voter.Address.StreetName);
            cmd.Parameters.AddWithValue("@MunicipalityId", voter.Address.MunicipalityId);
            cmd.Parameters.AddWithValue("@DistrictId", voter.Address.DistrictId);
            cmd.Parameters.AddWithValue("@Province", voter.Address.Province);

            // user credentials
            cmd.Parameters.AddWithValue("@UserName", voter.User.UserName);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@RoleId", voter.User.RoleId);

            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<DbResponse> ChangePasswordAsync(int userId, string plainPassword, CancellationToken cancellationToken)
        {
            // Hash the password using BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(plainPassword, 5);

            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("ChangeUserPassword");

            // Add parameters
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@newPassword", hashedPassword);

            return await dataAccess.ExecuteNonQueryAsync(cancellationToken);
        }



        public async Task<int> GetVoterCountAsync(CancellationToken cancellationToken)
        {
            int voterCount = 0;

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



        public async Task<int> ForgotPassword(string email, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("getUserId");

            cmd.Parameters.AddWithValue("@email", email);
            DataTable tbl= await dataAccess.ExecuteReader(cancellationToken);
            if (tbl.Rows.Count > 0)
            {
                return Convert.ToInt32( tbl.Rows[0][0]);
            }
            return -1;

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

        public async Task<List<VoterEnrollElection>> GetElectionByVoterIdAsync(int voterId, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            SqlCommand cmd = dataAccess.CreateCommand("GetElectionByVoterId");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@voterid", voterId);

             DataTable electionInfo = await dataAccess.ExecuteReader(cancellationToken);
            List<VoterEnrollElection> electionList = new List<VoterEnrollElection>();
            foreach (DataRow row in electionInfo.Rows)
            {
                electionList.Add(new VoterEnrollElection
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()
                });

            }
            return electionList;

        }





        public async Task<AuthResponse> VoterLogin(  string email, string password, IConfiguration configuration, CancellationToken cancellationToken)
        {
            DataAccess dataAccess = new DataAccess(_connectionString);
            string token = "";
            AuthResponse resp = new AuthResponse();

            //user.UserName = email;
            ////var RoleId = 1;
            //var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 5);
            //BCrypt.Net.BCrypt.EnhancedVerify(password, "$2a$05$XTbat7.nYiAiwjVqI8USEO9OewGXTYWZQLPJ9FH51r.y8O9uzg3.6");

            SqlCommand cmd = dataAccess.CreateCommand("ValidateUser");
            cmd.Parameters.AddWithValue("@Email", email);
            DataTable tbl = await dataAccess.ExecuteReader(cancellationToken);
            if (tbl.Rows.Count > 0) // email matching user exists
            {
                string hashedPassword = tbl.Rows[0]["password"].ToString();
                bool isPasswordMatch = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);

                if (isPasswordMatch) // password matches so user is valid for auth
                {
                    string roleName = tbl.Rows[0]["role_name"].ToString();
                    string user_id = tbl.Rows[0]["user_id"].ToString();
                    resp.Token = GenerateJwtToken(user_id, email, roleName, configuration);
                    resp.ExpiryDate = DateTime.Now.AddMinutes(30);
                    resp.voterid = tbl.Rows[0]["VoterId"].ToString();
                    resp.roleid = tbl.Rows[0]["role_id"].ToString();
                    resp.user_id = tbl.Rows[0]["user_id"].ToString();



                }
            }

            return resp;


        }


        private string GenerateJwtToken( string user_id, string username, string roleName, IConfiguration configuration)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role,roleName),
            new Claim("uid", user_id )
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Get<string>()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtIssuer = configuration.GetSection("Jwt:Issuer").Get<string>();

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    }
