﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using VotingAPI.Model;
using VotingAPI.Service;


namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;
        private readonly string _connectionString;

        public DataController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("VotingDb");
            _dataService = new DataService(connectionString); 
        }

        // Get all districts
        [HttpGet("districts")]
        
        public async Task<ActionResult<List<District>>> GetAllDistricts(CancellationToken cancellationToken)
        {
            try
            {
                var districts = await _dataService.GetAllDistrictsAsync(cancellationToken);
                return Ok(districts); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("municipalities")]
        public async Task<ActionResult<List<Municipality>>> GetMunicipalities([FromQuery] int? districtId, CancellationToken cancellationToken)
        {
            try
            {
                var municipalities = await _dataService.GetMunicipalitiesAsync(districtId, cancellationToken);
                return Ok(municipalities); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("ward")]
        public async Task<ActionResult<List<Ward>>> GetWard([FromQuery] int? municipalityId, CancellationToken cancellationToken)
        {
            try
            {
                var wards = await _dataService.GetWardAsync(municipalityId, cancellationToken);
                return Ok(wards);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Election/Type")]
        public async Task<ActionResult<List<ElectionType>>> GetElectionTypes(CancellationToken cancellationToken)
        {
            try
            {
                var electionTypes = await _dataService.GetAllElection(cancellationToken);
                return Ok(electionTypes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("Candidate/Position")]
        public async Task<ActionResult<List<CandidatesPosition>>> GetCandidatesPosition(CancellationToken cancellationToken)
        {
            try
            {
                var candidatesPositions = await _dataService.GetCandidatePosition(cancellationToken);
                return Ok(candidatesPositions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Centre/Detail")]
        public async Task<ActionResult<List<CandidatesPosition>>> GetCentreDetail(int electionId,CancellationToken cancellationToken)
        {
            try
            {
                var candidatesPositions = await _dataService.GetCentreDetailsAsync(electionId,cancellationToken);
                return Ok(candidatesPositions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpGet("Parties")]
        public async Task<IActionResult> GetParties()
        {
            var parties = await _dataService.GetAllPartiesAsync();
            return Ok(parties);
        }

        [HttpPost("Centre/Add")]
        public async Task<ActionResult<DbResponse>> AddAsync(Centre centre, CancellationToken cancellationToken)
        {
            try
            {
                var addedCentre = await _dataService.AddCentreAsync(centre, cancellationToken);

                return Ok(addedCentre);
            }
            catch (Exception ex) { 
                return BadRequest(new {Message = ex.Message});
            }

        }

        [HttpGet("Election/Municipality")]
        public async Task<IActionResult> GetMunicipalitiesByDistrict(int districtId, CancellationToken cancellationToken)
        {
            try
            {
                var municipalities = await _dataService.GetElectionMunicipality(districtId, cancellationToken);
                if (municipalities == null || municipalities.Count == 0)
                {
                    return NotFound(new { message = "No municipalities found for the given district." });
                }

                return Ok(municipalities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Election/Ward")]
        public async Task<IActionResult> GetWardByMunicipality(int municipalityId, CancellationToken cancellationToken)
        {
            try
            {
                var ward = await _dataService.GetElectionWard(municipalityId, cancellationToken);
                if (ward == null || ward.Count == 0)
                {
                    return NotFound(new { message = "No ward found for the given municipality." });
                }

                return Ok(ward);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Election/Centre/Detail")]
        public async Task<IActionResult> GetElectionCentreDetail(int electionCentreId, CancellationToken cancellationToken)
        {
            try
            {
                var details = await _dataService.GetElectionCentreDetailsAsync(electionCentreId, cancellationToken);
                if (details == null || details.Count == 0)
                {
                    return NotFound(new { message = "No details" });
                }

                return Ok(details);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("Candidate/Votes")]
        public async Task<IActionResult> GetCandidateVotes(int centreId, CancellationToken cancellationToken)
        {
            try
            {
                // Call the service method to get the candidate votes by the centre
                var candidateVotes = await _dataService.GetCandidateVotesByCentreAsync(centreId, cancellationToken);

                if (candidateVotes == null || candidateVotes.Count == 0)
                {
                    return NotFound(new { message = "No votes found for this centre." });
                }

                return Ok(candidateVotes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpGet("Centre/Name")]
        public async Task<IActionResult>GetName(int districtId, int municipalityId, int wardId, CancellationToken cancellationToken)
        {
            try
            {
                var name = await _dataService.GetCentreName(districtId, municipalityId, wardId, cancellationToken);
                if (name == null || name.Count == 0)
                {
                    return NotFound(new { message = "No centre name found" });
                }

                return Ok(name);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }


    }
}
