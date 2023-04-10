using Microsoft.AspNetCore.Identity;

namespace API.Entities
{   //many-to-many relationships between appuser and approle where 
    //each role have many users inside a role and each user can have/be a member of many roles
    //Able to retain maximum control over join tables than getting Entity framework to do it.
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}