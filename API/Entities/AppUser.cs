using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // Note: ASP.Net core Indentity will be responsible for taking over custom hash in accountcontroller to hash(10000 iterations), salt and storing of password in database
    //it takes cares of the id, username, password and passwordhash
    public class AppUser : IdentityUser<int>
    {

        public DateOnly DateOfBirth { get; set; } // create extension API to calculate age of birth of each user

        public string KnownAs { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        //if there's entity or a type(Photo) added to another class(AppUser), it is referred to an entity framework - navigation property 
        public List<Photo> Photos { get; set; } = new ();
        // Avoid repeated cycle from getting methods between AppUser and Photo class (create seperate DTO class with appropriate properties)

        public List<UserLike> LikedByUser { get; set; }
        public List<UserLike> LikedUsers { get; set; }

        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; } // navigation property to the join table (appuser collection)


    }


}