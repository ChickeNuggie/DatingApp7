using API.Data;
using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{

    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<IEnumerable<AppUser>> GetUserAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
        //Return MemberDto directly from repository to display appropriate and efficient informations.
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser);
        Task<string> GetUserGender(string username);
        //get user by the photo Id to check if user has photos set to main.
        Task<AppUser> GetUserByPhotoId(int photoId); 



    }
}