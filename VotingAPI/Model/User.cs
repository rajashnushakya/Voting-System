using Microsoft.AspNetCore.Identity;

namespace VotingAPI.Model
{
    public class AppUser : IdentityUser<int>
    {
    }

    public class Role : IdentityRole<int>
    {
        public int Id{ get; set; }

        public string Name { get; set; }

        void a()
        {
            AppUser u = new AppUser();  
        }
    }


    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string voterid { get; set; }
    }

}
