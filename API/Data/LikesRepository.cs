using API.Controllers;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        //Find UserLike entity and match with the primary keys: combination of 2 intergers params
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        // //get the list of user that the user has liked or has been liked.
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {   // access context and Users table, orderby username and store in query before execution
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            // check if users have liked other users
            if (likesParams.Predicate == "liked")
            {   
                //Filter and only select users that are inside this list (users liked other users)
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(likes => likes.TargetUser);
            }

            // check if other users have liked the current user
            if (likesParams.Predicate == "likedBy")
            {   
                //Filter and only select users that are inside this list (users liked other users)
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(likes => likes.SourceUser);
            }
            //inject into Like DTO and map the properties from users query
            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            }); 
            
            // returns a pagedlist and not executing
            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        } 

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            //check to see if a user already has been liked by another user.
            return await _context.Users
                    .Include(x => x.LikedUsers)
                    .FirstOrDefaultAsync(x => x.Id == userId); 
        }
    }
}