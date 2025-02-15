using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using VotingAPI.Model;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ElectionController : ControllerBase
    {
        private readonly string _connectionString;

        public ElectionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VotingDb");
        }

        [HttpPost]
        public async Task<DbResponse> AddAsync(Election election, CancellationToken cancellationToken) {
            ElectionService electionService = new ElectionService(_connectionString);
            return await electionService.AddElectionAsync(election, cancellationToken);
            
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync( Election election, CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);

            var elections = await electionService.GetElectionAsync(election, cancellationToken);

            if (elections.Count == 0)
            {
                return NotFound("No elections found.");
            }

            return Ok(elections);
        }
    }
}
