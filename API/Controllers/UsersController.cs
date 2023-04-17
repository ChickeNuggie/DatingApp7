using System.Security.Claims;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Server(WebAPI) backend system (brain of the computer)
namespace API.Controllers
{
    [Authorize] // allow endpoints to be authenticaed against.

    // [ApiController]//API controller attribute route
    // [Route("api/[controller]")]  //API route, GET /api/users (First part of the name of the controller after localhost:5000/ etc.)
    //1. Make request to this api controller above,
    //2. UserController class will be created by entity framework

    // Inheritance from BaseApiController
    public class UsersController : BaseApiController // every controller needs to derieve from controller based class
    {


        //MVC - Model, View, Controller
        // 'Model' 1-1 mapping template for each table of database that consist of columns and attribute.
        // FYI : 'View' - contains final data to view on front-end page. 
        private readonly IMapper _mapper;

        //dependency injection
        // allow access to database/sessions and  extract out users using entity framework query (translated into sql query) and return the users' data from API endpoints to the client.
        // 3. Any dependency added inside constructor are also going to be created.
        // Inject interface Imapper
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _uow;
        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService) // create respository interface which then have available session with the UserRepository to get all data from database. 
        {
            _uow = uow;
            _photoService = photoService;
            _mapper = mapper;
        }
    
        // Endpoints attirbute to have HTTP method that's being used to make request
        // It gets method that's requesting API users in this case (GET /api/users)
        // It gets request from database 
        // Get request returns 200 OK response
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)  // asynchronous code, specify from query string for API to find the parameters
        // ActionResult allows to wrap responses inside (i.e. badrequest(), NotFound() etc.)
        // It is a method in a controller class that handles incoming HTTP requests and returns an HTTP response to the client.
        // It provides a way to encapsulate the result of an action method, including the HTTP status code, response body, and other HTTP headers.
        // It is is an abstract class, which means that it cannot be instantiated directly.
        // Instead, you can use one of its derived classes to return a specific type of HTTP response, such as:
        //-  OkObjectResult for a  successful response with a JSON payload or NotFoundResult for a 404 error.
        // This makes it easier to write testable and maintainable code.

        {//optimizing code by implmenting specific method just to get use's gender instead of returning whole user database.
            var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername(); // populate current user name in user parameter

            // Default gender
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _uow.UserRepository.GetMembersAsync(userParams); // asynchronous await = wait for it and notify when its been compelted.
        // get List of the User's Database (IEnumerable = C# array)  
        // easier than writing query, making connection to database, ensure connections available and then writing sql, get it back and map it to objects that we want to return
            
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));
            
            return Ok(users);        
        }


        [HttpGet("{username}")] // Spcify username of the User to Get correct user database from the repository
        public async Task<ActionResult<MemberDto>> GetUser(string username) // asynchronous code
        {

        // synchornous code goes on a single thread to effectively make request to database and then its waiting (await) which unable to do other tasks as its block till request comes back from database and then it can return to the user.
        // return _context.Users.Find(id); // It finds entity with the given primary key

         // asynchronous code allows to get other queries than from incoming request from web sever (no await) and isn't blocked whilist waiting for current query on database to return.
        // may add abit mild extra noise to the code.

         return  await _uow.UserRepository.GetMemberAsync(username);

        }

        // Update the endpoints of the users' profile
        //Update response return no content 
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
        {
            // Use ClaimsPrinipal Extensions to Find and match nameId (i.e. 'lisa') in the first claims property inside the security JWS token claims. Optional changing to prevent null exception 
            // Ensure that photo is being loaded when getting from userrepository as photos will be counted.
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername()); // Entity framework tracks the username(i.e. 'lisa') and go to repository to get the username

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user); // update DTO objects to be written inside user's properties

            if (await _uow.Complete()) return NoContent();// save the changes to database and return 204 ok status

            return BadRequest("Failed to update user"); // if there's no changes to be saved.
        } 

        // When creating new resource on sever, returns '201 created' which provides header response to find this new resource.
        [HttpPost("add-photo")]  //stores data from front-end client-side to the database
        // Update a user object and add photo to their collection in the database.
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
        // create extension method to use find username based on claim principal extensions: nameidentifier to use across classess.
         var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

         if (user == null) return NotFound();

        // Add photo to cloudinary services using Photoservice class
         var result = await _photoService.AddPhotoAsync(file);

         if(result.Error != null) return BadRequest(result.Error.Message);

        // If there is result, create new upload Photo into database
         var photo = new Photo
         {  // path to the image on cloudinary storage
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
         };

         //Set the uploaded photo to main photo if no photos exist in database.
         if (user.Photos.Count == 0) photo.IsMain = true;
        // User entity Framework is now tracking the 'user' list in the memory
         user.Photos.Add(photo);

         // if changes is saved to database and is working, return mapping into PhotoDTO from photo
        // Go through request for user (user login) based on their username and get their exact location of newly uploaded url images created on server.
        // This could prevent unauthorized access to individual photos. 
         if (await _uow.Complete())
         {
            //created uploads upon action tasks above.
            //Get User via User api "user/username" to get root value = name of the user.
            // Map from created parameter photo and pass back to PhotoDto
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
         }

         return BadRequest("Problem adding photo.");
        }

        // Set main photo baesd on the photo id
        [HttpPut("set-main-photo/{photoId}")] 
        public async Task<ActionResult> SetMainPhoto(int photoId) 
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if(user == null) return NotFound();
            // find photo id from users' photo object
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault( x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _uow.Complete()) return NoContent();

            return BadRequest("Problem setting main photo");
            }

        // delete photos from backend API
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null )
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest (result.Error.Message);
            }
            // Entity Framework tracks use entity for any changes saved to database
            user.Photos.Remove(photo);
            
            // correct response to return for a http delete request/method
            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting photo");
        }
    }
}