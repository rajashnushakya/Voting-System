using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Model;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VoterController : ControllerBase
    {
        private readonly string _connectionString;

        public IConfiguration Configuration { get; }

        public VoterController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VotingDb");
            Configuration = configuration;
        }


        [HttpPost]
        public async Task<DbResponse> RegisterAsync(Voter voter, CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            return await voterService.RegisterAsync(voter, cancellationToken);


        }

        [HttpGet]
        public async Task<IActionResult> CountAsync(CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            var voterCount = await voterService.GetVoterCountAsync(cancellationToken);


            return Ok(voterCount);
        }

        [HttpGet]
        public async Task<IActionResult> GetElectionByVoterIdAsync(int voterId, CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            var result = await voterService.GetElectionByVoterIdAsync(voterId, cancellationToken);

            if (result.Rows.Count == 0)
            {
                return NotFound("No election found for the provided voter ID.");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<DbResponse> VoterEnrollmentAsync(int voterId, int electionId, CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            return await voterService.VoterEnrollment(electionId, voterId, cancellationToken);

        }

        [HttpPost]
        public async Task<AuthResponse> LoginAsync( [FromBody]LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            return await voterService.VoterLogin(loginRequest.email, loginRequest.password, Configuration, cancellationToken);
        }



        [HttpGet]
        public string Test()
        {
            return "success";
        }

        [HttpPut]
        public async Task<DbResponse> ChangePassword([FromBody] ForgotPassword forgotPassword , CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            return await voterService.ChangePasswordAsync(Convert.ToInt32(forgotPassword.userId), forgotPassword.newPassword, cancellationToken);
        }

        [HttpGet]
        public async Task<int> GetUserId(string email,CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            var user_id = await voterService.ForgotPassword(email,cancellationToken);


            return user_id;
        }

    }
}
