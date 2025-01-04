using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VotingAPI.Model;
using VotingAPI.Service;

namespace VotingAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpPost]
        public async Task<IdentityResult> RegisterAsync(AppUser user, CancellationToken cancellationToken)
        {
            AppUserStore store = new AppUserStore();
            IdentityResult result =  await store.CreateAsync(user, cancellationToken);

            return result;

        }
    }
}
