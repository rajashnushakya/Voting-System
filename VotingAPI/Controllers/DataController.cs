using Microsoft.AspNetCore.Mvc;
using VotingAPI.Service;
using VotingAPI.Model;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;

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

        [HttpGet("Parties")]
        public async Task<IActionResult> GetParties()
        {
            var parties = await _dataService.GetAllPartiesAsync();
            return Ok(parties);
        }
    }
}
