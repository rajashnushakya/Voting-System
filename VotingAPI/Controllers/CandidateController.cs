using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using VotingAPI.Model;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly string _connectionString;

        public CandidateController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VotingDb");
        }

        [HttpPost()]
        public async Task<DbResponse> RegisterCandidate(Candidate candidate, CancellationToken cancellationToken)
        {
            CandidateService candidateService = new CandidateService(_connectionString);
            return await candidateService.AddCandidateAsync(candidate, cancellationToken);  
        }


        [HttpGet("Candidates")]
        public async Task<ActionResult<DbResponse>> GetCandidate(int voterId, CancellationToken cancellationToken)
        {
            try
            {
                CandidateService candidateService = new CandidateService(_connectionString);
                var candidates = await candidateService.GetCandidatesAsync(voterId, cancellationToken);
                return Ok(candidates);
            }
            catch(Exception ex) 
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
        [HttpPost("CandidateCentre")]
        public async Task<DbResponse> CandidateCentreAsync(List<CandidateCentre> candidateCentre, CancellationToken cancellationToken)
        {
            CandidateService candidateService = new CandidateService(_connectionString);
            return await candidateService.centreCandidateAsync(candidateCentre, cancellationToken);

        }

        [HttpGet("GetCandidateByElection")]
        public async Task<ActionResult<List<Candidate>>> GetCandidatesByElectionId(int electionId, CancellationToken cancellationToken)
        {
            try
            {
                var candidateService = new CandidateService(_connectionString);
                var candidates = await candidateService.GetCandidatesByElectionIdAsync(electionId, cancellationToken);

                if (candidates == null || candidates.Count == 0)
                {
                    return NotFound("No candidates found for this election ID.");
                }

                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("GetCandidateByElectionCentre")]
        public async Task<ActionResult<List<CandidateWithParty>>> GetCandidateByElectionCentre(int centreId, CancellationToken cancellationToken)
        {
            try
            {
                var candidateService = new CandidateService(_connectionString);
                var candidates = await candidateService.GetCandidatesByCentreIdAsync(centreId, cancellationToken);

                if (candidates == null || candidates.Count == 0)
                {
                    return NotFound("No candidates found for this election centre ID.");
                }

                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
