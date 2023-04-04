 // Only sends portion of information to user without passwordhash and passwordsalt
namespace API.DTO
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        // save time making a request to API to find out what this is from client
        // pass down gender properties when log in with user DTO object and store in local storage with other properties inside DTO.
        public string Gender { get; set; } 
    }
}