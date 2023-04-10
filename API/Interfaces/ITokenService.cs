using API.Entities;

namespace API.Interfaces
{
    // Force into contract for any other classess that implement this interface.
    //// able to isolate test without implementing other context but only classes that implements this.
    public interface ITokenService 
    {
     Task<string> CreateToken(AppUser user);   
    }
}