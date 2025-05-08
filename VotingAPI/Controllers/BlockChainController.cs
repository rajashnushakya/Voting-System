using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Text;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BlockChainController : ControllerBase
    {

        [HttpPost]
        public async Task<string> SendVote(VoteDto vote)
        {
            BlockChainService service = new BlockChainService();
            return await service.CastVoteAsync(vote);
        }

        [HttpGet]
        public async Task<int> GetTotalVotesAsync(int electionId)
        {
            BlockChainService blockChainService = new BlockChainService();
            return await blockChainService.GetTotalVotesAsync(electionId);
        }

        [HttpGet]
        public async Task<int> CountVote(int electionId, int candidateId)
        {
            BlockChainService blockChainService = new BlockChainService();
            return await blockChainService.GetVoteCountAsync(electionId, candidateId);
        }

        [HttpGet]
        public async Task<VoteDto> ReadVote(int index)
        {
            BlockChainService blockChainService = new BlockChainService();
            return await blockChainService.GetVoteRecordAsync(index);
        }
    }
}
