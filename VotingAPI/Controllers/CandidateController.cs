﻿using Microsoft.AspNetCore.Http;
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

        [HttpPost]
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

    }
}
