using DbUp.Engine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private IConfiguration _configuration { get; }

        public DatabaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
        [HttpGet]
        public DatabaseUpgradeResult? Synchronize()
        {
            return DbMaintainer.Maintain(_configuration.GetConnectionString("VotingDb"));
        }
    }
}
