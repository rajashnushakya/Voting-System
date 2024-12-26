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

}
