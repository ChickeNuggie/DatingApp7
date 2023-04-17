using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //After creating repository, entity and migration,
    //Create controller to use functionality inside the repository.
    //Note: BaseAPI include API controller attributes and route that uses controller
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;

     public LikesController(IUnitOfWork uow)
        {
            _uow = uow;

        }   

        //create new resource when user likes another user
        [HttpPost("{username}")]
        //Update join table by creating new entry in the table and not actually creating new/update user
        public async Task<ActionResult> AddLike(string username)
        {   //user that likes other user
            var sourceUserId = User.GetUserId();
            var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);
        
            //Defensive programming by raising and throw exception.
            if (likedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You have already liked this user!");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            
            // Create entry in the userlikes join table
            sourceUser.LikedUsers.Add(userLike);

            if (await _uow.Complete()) return Ok(); 
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        // predicate to choose if to send back request of the 'liked users' or 'liked by users' from API to database.
        //require to specify from query for non-query string (i.e objects) as query object are easily binded to
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _uow.LikesRepository.GetUserLikes(likesParams);

            //pagination header to filter and query specific properties.
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }
}