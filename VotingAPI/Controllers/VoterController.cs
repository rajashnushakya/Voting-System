﻿using Microsoft.AspNetCore.Authorization;
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

        public VoterController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("VotingDb");
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

        [HttpPost]

        [HttpPost]
        public async Task<DbResponse> VoterEnrollmentAsync(int voterId, int electionId, CancellationToken cancellationToken)
        {
            VoterService voterService = new VoterService(_connectionString);
            return await voterService.VoterEnrollment(electionId, voterId, cancellationToken);

        }


        [HttpGet]
        public string Test()
        {
            return "success";
        }
    }
}
