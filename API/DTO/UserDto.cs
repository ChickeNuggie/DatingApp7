 // Only sends portion of information to user without passwordhash and passwordsalt
namespace API.DTO
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
    }
}