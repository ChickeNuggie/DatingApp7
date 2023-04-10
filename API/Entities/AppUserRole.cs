using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    //Represent join tables  between appuser and approles.
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }    
        public AppRole Role { get; set; }
    }
}