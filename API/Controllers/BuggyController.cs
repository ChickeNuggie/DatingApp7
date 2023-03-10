// reeturn HTTP error responses to the client, and it's a controller.
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context; // initiliaze field parameter
        }
    
        [Authorize] // ensure 401 unthoarized  from httpget("auth") when user is not authenticating GetSecret end point.
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
         {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
         {
            var thing = _context.Users.Find(-1); // attempt to find user that does not exist

            if(thing == null) return NotFound(); // return notfound htp response

            return thing;
        }


        [HttpGet("server-error")]
        public ActionResult<string> GetSrverError()
         { // try and catch potential exception
         // Silently handlded expcetion in development page and straight away output back to client the error message.

            var thing = _context.Users.Find(-1); // attempt to find user that does not exist

            var thingToReturn = thing.ToString(); // throw an exception error when convert null string which generates no exception error
            //Aim: to create eception from this method in the controller.
            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
         {
            return BadRequest("This was not a good request");
        }
    }
}
