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
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            var elections = await electionService.GetActiveElectionAsync(cancellationToken);

            if (elections == null || elections.Count == 0)
            {
                return NotFound("No active elections found.");
            }

            return Ok(elections);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllElectionAsync(CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            var elections = await electionService.GetAllElectionAsync(cancellationToken);

            if (elections == null || elections.Count == 0)
            {
                return NotFound("No elections found.");
            }

            return Ok(elections);
        }


        [HttpGet]
        public async Task<IActionResult> CountAsync(CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            var electionCount= await electionService.GetElectionCountAsync(cancellationToken);


            return Ok(electionCount);
        }

        [HttpPost]
        public async Task<DbResponse> CentreAsync(List<ElectionCentre> electionCentre, CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            return await electionService.AddElectionCentreAsync(electionCentre, cancellationToken);

        }

        [HttpPost]
        public async Task<DbResponse> EndElectionAsync(int electionId, CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            return await electionService.EndElectionAsync(electionId, cancellationToken);

        }

        [HttpPost]
        public async Task<DbResponse> StartElectionAsync(int electionId, CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            return await electionService.StartElectionAsync(electionId, cancellationToken);

        }


        [HttpGet]
        public async Task<IActionResult> ElectionCentreAsync(int electionId, CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            var response = await electionService.GetElectionCentre(electionId, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        public async Task<DbResponse> VoterElectionCentreEnrollment(int centreId, int voterId ,CancellationToken cancellationToken)
        {
            ElectionService electionService = new ElectionService(_connectionString);
            return await electionService.InsertElectionCentreVoterAsync(centreId, voterId, cancellationToken);

        }


    }
}
