using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Note API service job is stateless where it receive request, do logic based on request and return a response.
//API does not remember a session that a client has (not designed to save responses)
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        //Create and post resource on the sever which creates endpoint that the user can use to register their acocunt.
        [HttpPost("register")] // POST: api/account/register
        // POST: api/account/register?username=dave&password=pwd, query string is not an object and able to bind two string argument.
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) // object parameter passed in to avoid using query where info is shown in url
        {   
            //Check if username exists
            if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto); //Map properties into app user from registerDto.


            // Update user object values  
            user.UserName = registerDto.UserName.ToLower(); // to compare same object string values due to case sensitivity

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            //add users' member roles upon registering method.
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        // Login page
        [HttpPost("login")] // POST: api/account/login
        // takes a LoginDto object as input and returns an ActionResult<UserDto>.
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {       
            // User's password Hash (byte array)
            // The user's information is retrieved from the database context using the provided loginDto.Username as a filter.
            // By default, Entity framework does not load related entities
            var user = await _userManager.Users
            .Include(p => p.Photos) // load photos to ensure user.Photos not null and check its property if isMain and access the URL
            .SingleOrDefaultAsync(x => // return result that matches the username
            x.UserName == loginDto.Username); // 'Find' requires primary key but username is not thus, use either First/SingleOrDefaultAsync

            // Check to see if user exist in data base
            if(user == null) return Unauthorized("invalid username");
            //check password from user against dto database.
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid Password");

            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
            //Overall, this method checks the user's input against the stored user data in the database and returns a token if the input is valid.
        }   

        private async Task<bool>  UserExists(string username)// asynchronous to enter database and check if user has already registered
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower()); // to compare same object string values due to case sensitivity
        }
    }
}