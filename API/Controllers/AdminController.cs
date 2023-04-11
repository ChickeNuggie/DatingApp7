using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   //Adding policy based on authorization 
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")] //get request from database html to get below's method
        public async Task<ActionResult> GetUsersWithRoles()
        {
            //get a list of users
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                // access related "roles" tables of user roles table.
                .Select(u => new // return annoymous object with user id, name and roles.
                {
                    u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                    
                })
                .ToListAsync();
            return Ok(users);
        }

        //Allow admin to edit users' roles. Its just hiding information behind the curtain.
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            //split list of comma separaters roles.
            var selectedRoles = roles.Split(",").ToArray();
            //look for username in usermanager
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();
            //get existing user roles inside users
            var userRoles = await _userManager.GetRolesAsync(user);

            //add roles into users that is not currently in except when users are already a member of a user role.(have roles already)
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to to roles");

            //Account the removal of roles from users by the admin.
            //any roles that user was already inside of that are not contained inside the selected roles list created above will by removed from that particular role
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
             
             if (!result.Succeeded) return BadRequest("Failed to remove from roles");

             return Ok(await _userManager.GetRolesAsync(user));
        
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}