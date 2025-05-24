using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Web3;
using System.Numerics;

namespace VotingAPI.Service
{
    public class BlockChainService
    {
        private readonly string rpcURL = "http://127.0.0.1:7545";
        private readonly string accountPrivateKey = "0x043f8724f44a870cbb624f10701282010d6258e7012c02fc0ae72fd1decf6bad";
        private readonly string contractAddress = "0xB1e167Df0bF4f1B245aBF578B41B2C804E20B0dC";

        private Web3 web3;
        public BlockChainService()
        {
            web3 = new Web3(new Nethereum.Web3.Accounts.Account(accountPrivateKey), rpcURL);
        }

        private static long ToUnixTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        private static DateTime ToDateTime(long unixDateTime)
        {
            DateTime localTime = DateTimeOffset.FromUnixTimeSeconds(unixDateTime).ToLocalTime().DateTime;
            return localTime;
        }

        /// <summary>
        /// Cast a vote for a candidate in an election
        /// </summary>
        /// <param name="vote">A vote detail <see cref="VoteDto"/></param>
        /// <returns></returns>
        public async Task<string> CastVoteAsync(VoteDto vote)
        {
            try
            {
                var handler = web3.Eth.GetContractTransactionHandler<CastVoteFunction>();

                var voteFunction = new CastVoteFunction
                {
                    ElectionId = vote.ElectionId,
                    VoterId = vote.VoterId,
                    CandidateId = vote.CandidateId,
                    CentreId = vote.CenterId,
                    VoteTime = ToUnixTimestamp(vote.VoteTime)
                };

                var receipt = await handler.SendRequestAndWaitForReceiptAsync(contractAddress, voteFunction);
                return receipt.TransactionHash;
            }
            catch(SmartContractRevertException ex)
            {
                return ex.Message; //if voter has already voted will return: Voter already voted in this election
            }
            //any other exception
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        /// <summary>
        /// Read all votes that has been casted in a election for a specific candidate.
        /// If the value return by this method doesn't match our database election/candidate grouped vote count there is vote tempering
        /// </summary>
        /// <param name="electionId">An election where a candidate is registered</param>
        /// <param name="candidateId">A candidate whose vote needs to be count</param>
        /// <returns></returns>
        public async Task<int> GetVoteCountAsync(int electionId, int candidateId)
        {
            var handler = web3.Eth.GetContractQueryHandler<GetVoteCountFunction>();

            var function = new GetVoteCountFunction
            {
                ElectionId = electionId,
                CandidateId = candidateId
            };

            BigInteger result =  await handler.QueryAsync<BigInteger>(contractAddress, function);
            return (int)result;
        }

        /// <summary>
        /// Get total votes that was casted
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetTotalVotesAsync(int electionId)
        {
            var handler = web3.Eth.GetContractQueryHandler<GetTotalVotesFunction>();
            var function = new GetTotalVotesFunction { ElectionId = electionId };
            BigInteger result =  await handler.QueryAsync<BigInteger>(contractAddress, function);
            return (int)result;
        }

        public async Task<VoteDto> GetVoteRecordAsync(int index)
        {
            var handler = web3.Eth.GetContractQueryHandler<GetVoteRecordFunction>();
            var function = new GetVoteRecordFunction
            {
                Index = index
            };
            //return await handler.QueryDeserializingToObjectAsync<GetVoteRecordFunction, VoteOutputDto>(function, contractAddress);

            VoteOutputDto result =  await handler.QueryDeserializingToObjectAsync<VoteOutputDto>(function, contractAddress);
            return new VoteDto
            {
                CandidateId = (int)result.CandidateId,
                CenterId = (int)result.CentreId,
                ElectionId = (int)result.ElectionId,
                VoterId = (int)result.VoterId,
                VoteTime = ToDateTime((long)result.VoteTime),
            };
        }

        /// <summary>
        /// Gets the current total number of blocks on the blockchain.
        /// </summary>
        /// <returns>The latest block number (total blocks).</returns>
        public async Task<BigInteger> GetBlockCountAsync()
        {
            try
            {
                var latestBlockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                return latestBlockNumber.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching block count: {ex.Message}");
                return -1;
            }
        }

    }





    [Function("castVote")]
    public class CastVoteFunction : FunctionMessage
    {
        [Parameter("uint256", "electionId", 1)]
        public BigInteger ElectionId { get; set; }


        [Parameter("uint256", "voterId", 1)]
        public BigInteger VoterId { get; set; }


        [Parameter("uint256", "candidateId", 1)]
        public BigInteger CandidateId { get; set; }


        [Parameter("uint256", "centerId", 1)]
        public BigInteger CentreId { get; set; }


        [Parameter("uint256", "voteTime", 1)]
        public BigInteger VoteTime { get; set; }
    }


    [Function("getVoteCount", "uint256")]
    public class GetVoteCountFunction : FunctionMessage
    {
        [Parameter("uint256", "electionId", 1)]
        public BigInteger ElectionId { get; set; }

        [Parameter("uint256", "candidateId", 2)]
        public BigInteger CandidateId { get; set; }
    }

    [Function("getTotalVotes", "uint256")]
    public class GetTotalVotesFunction : FunctionMessage
    {
        [Parameter("uint256", "electionId", 1)]
        public BigInteger ElectionId { get; set; }
    }

    [Function("getVoteRecord", typeof(VoteOutputDto))]
    public class GetVoteRecordFunction : FunctionMessage
    {
        [Parameter("uint256", "index", 1)]
        public BigInteger Index { get; set; }
    }


    [FunctionOutput]
    public class VoteOutputDto : IFunctionOutputDTO
    {
        [Parameter("uint256", "electionId", 1)]
        public BigInteger ElectionId { get; set; }

        [Parameter("uint256", "voterId", 2)]
        public BigInteger VoterId { get; set; }

        [Parameter("uint256", "candidateId", 3)]
        public BigInteger CandidateId { get; set; }

        [Parameter("uint256", "centreId", 4)]
        public BigInteger CentreId { get; set; }

        [Parameter("uint256", "voteTime", 5)]
        public BigInteger VoteTime { get; set; }
    }


    public class VoteDto
    {
        /// <summary>
        /// A election where a voting is being performed
        /// </summary>
        public int ElectionId { get; set; }

        /// <summary>
        /// An eligible person who can vote in an election
        /// </summary>
        public int VoterId { get; set; }

        /// <summary>
        /// A registered candidate from a political party or individual who has registered as a candidate
        /// </summary>
        public int CandidateId { get; set; }

        /// <summary>
        /// A voting place where voters will cast their vote 
        /// </summary>
        public int CenterId { get; set; }

        /// <summary>
        /// The exact date and time when a voter has casted their vote
        /// </summary>
        public DateTime VoteTime { get; set; } 
    }

}