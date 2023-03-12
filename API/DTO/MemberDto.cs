namespace API.DTO
{
    public class MemberDto
    {
        public int Id { get; set; } // User's ID, defasult constructor value is 0.
        public string UserName { get; set; } // ? is optional constructor as stirng by default requires a non-null value. 
        //User are only able to set one photo as the main photo (i.e. Display photo etc.)
        public string PhotoUrl { get; set; }
        public int Age{ get; set; }

        public string KnwonAs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        //if there's entity or a type(Photo) added to another class(AppUser), it is referred to an entity framework - navigation property 
        //Able to set many photos for each user
        public List<PhotoDto> Photos { get; set; }
    }
}