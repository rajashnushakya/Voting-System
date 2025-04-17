﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VotingAPI.Service;
using VotingAPI.Model;

[ApiController]
[Route("api/[controller]")]
public class CandidateVoteController : ControllerBase
{
    private readonly CandidateVoteService _candidateVoteService;


    public CandidateVoteController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("VotingDb");
        _candidateVoteService = new CandidateVoteService(connectionString);
    }

    [HttpPost("Vote")]
    public async Task<IActionResult> Vote([FromBody] Vote request)
    {
        try
        {
            // Call the service method to insert the vote into the database
            await _candidateVoteService.InsertCandidateVoteAsync(
                request.CandidateId,
                request.VoterId,
                request.ElectionId,
                request.ElectionCentreId
            );

            return Ok("Vote successfully recorded.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("GetVotesByVoter/{voterId}")]
    public async Task<IActionResult> GetVotesByVoterId(int voterId)
    {
        try
        {
            var voteDetails = await _candidateVoteService.GetVotesByVoterIdAsync(voterId);

            if (voteDetails == null || voteDetails.Count == 0)
            {
                return NotFound("No votes found for this voter.");
            }

            return Ok(voteDetails);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
