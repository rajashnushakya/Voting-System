using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Model;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartyController : ControllerBase
    {
        private readonly PartyService _partyService;
        private readonly string _connectionString;

        public PartyController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("VotingDb");
            _partyService = new PartyService(connectionString);
        }
        [HttpGet]
        public async Task<ActionResult<List<Party>>> GetParties(CancellationToken cancellationToken)
        {
            try
            {
                var parties = await _partyService.GetParties(cancellationToken);
                return Ok(parties);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }


}
