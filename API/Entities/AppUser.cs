using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } // User's ID, defasult constructor value is 0.
        public string UserName { get; set; } // ? is optional constructor as stirng by default requires a non-null value. 
        // Alternatively, disable null control in API.csproj file 'Nullable'
        
        // adding new entities should also create additional columns to accommodate these properties via new migrations.
        // and update database accordingly
        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateOnly DateOfBirth { get; set; } // create extension API to calculate age of birth of each user

        public string KnwonAs { get; set; }

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

        //Automapper needs to get full entity in order to use this method inside thus, causing big query to still occur and result in returning passwordhash,salt, etc.
        // public int GetAge() 
        // {
        //     return DateOfBirth.CalculateAge();
        // }

    }


}