using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    // properties to receive from users.
    public class RegisterDto
    {
        [Required] 
        public string UserName { get; set; }
        [Required] 
        public string Password { get; set; }
    }
}