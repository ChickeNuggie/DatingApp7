using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{   //Adding policy based on authorization 
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _uow;
        private readonly IPhotoService _photoservice;
        public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IPhotoService photoservice)
        {
            _photoservice = photoservice;
            _uow = uow;
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
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photos = await _uow.PhotoRepository.GetUnapprovedPhotos();

            return Ok(photos);
        }

        //Admin controller to approve a photo by an admin/moderator based on photo Id.
        [Authorize(Policy = "ModeratePhotoRole")] // identity service exteension on the authorized role management.
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {   //retrieve photo from unit of work - photo repository
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound("Could not find photo");

            //Only admin/moderator can approve
            photo.IsApproved = true;

            //get photo id from user repo to check if user set the photo to main photo.
            var user = await _uow.UserRepository.GetUserByPhotoId(photoId);

            //if the photo of the user is not set to main yet, set it to main photo after approved. 
            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

            // similar to save async method in IUnitOfWork
            await _uow.Complete(); 

            return Ok();
        }

        //Admin controller to approve a photo by an admin/moderator based on photo Id.
        [Authorize(Policy = "ModeratePhotoRole")] // identity service exteension on the authorized role management.
        //return an HTTP status code in the form of an ActionResult object.
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {//retrieves the photo object associated with the given photoId 
            var photo = await _uow.PhotoRepository.GetPhotoById(photoId);

            if(photo.PublicId != null)
            {   //use photo services to delete photos from cloudinary database based on photo's public ID.
                //The PhotoService is expected to return a result object that contains a "Result" property
                var result = await _photoservice.DeletePhotoAsync(photo.PublicId);

                // if deletephotoasync successful(removed from cloudinary database), method calls the RemovedPhoto method of the PhotoRepository to mark the photo as removed. 
                if (result.Result == "ok")
                {   
                    _uow.PhotoRepository.RemovedPhoto(photo);
                }
            }
            else
            { // The method still calls RemovedPhoto but doesn't remove the photo from the cloudinary database.
                _uow.PhotoRepository.RemovedPhoto(photo);
            }

        await _uow.Complete(); //save the changes to the database. 

        return Ok();//returns an "Ok" status code to indicate that the operation was successful.
        

        }
    }
}
