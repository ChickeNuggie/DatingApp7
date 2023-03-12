using API.DTO;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IUserRepository _userRepository;
        private readonly IMapper _mapper;

        //dependency injection
        // allow access to database/sessions and  extract out users using entity framework query (translated into sql query) and return the users' data from API endpoints to the client.
        // 3. Any dependency added inside constructor are also going to be created.
        // Inject interface Imapper
        public UsersController(IUserRepository userRepository, IMapper mapper) // create respository interface which then have available session with the UserRepository to get all data from database. 
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
    
        // Endpoints attirbute to have HTTP method that's being used to make request
        // It gets method that's requesting API users in this case (GET /api/users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()  // asynchronous code
        // ActionResult allows to wrap responses inside (i.e. badrequest(), NotFound() etc.)
        // It is a method in a controller class that handles incoming HTTP requests and returns an HTTP response to the client.
        // It provides a way to encapsulate the result of an action method, including the HTTP status code, response body, and other HTTP headers.
        // It is is an abstract class, which means that it cannot be instantiated directly.
        // Instead, you can use one of its derived classes to return a specific type of HTTP response, such as:
            //-  OkObjectResult for a successful response with a JSON payload or NotFoundResult for a 404 error.
        // This makes it easier to write testable and maintainable code.
        {
            var users = await _userRepository.GetMembersAsync(); // asynchronous await = wait for it and notify when its been compelted.
        // get List of the User's Database (IEnumerable = C# array)  
        // easier than writing query, making connection to database, ensure connections available and then writing sql, get it back and map it to objects that we want to return
            return Ok(users);        
        }

        [HttpGet("{username}")] // Spcify username of the User to Get correct user database from the repository
        public async Task<ActionResult<MemberDto>> GetUser(string username) // asynchronous code
        {

        // synchornous code goes on a single thread to effectively make request to database and then its waiting (await) which unable to do other tasks as its block till request comes back from database and then it can return to the user.
        // return _context.Users.Find(id); // It finds entity with the given primary key

         // asynchronous code allows to get other queries than from incoming request from web sever (no await) and isn't blocked whilist waiting for current query on database to return.
        // may add abit mild extra noise to the code.

         return  await _userRepository.GetMemberAsync(username);

        }
    }
}