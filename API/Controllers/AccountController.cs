using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Note API service job is stateless where it receive request, do logic based on request and return a response.
//API does not remember a session that a client has (not designed to save responses)
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
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

            // If username does not exist, create new instance in memory and then create App user.
            using var hmac = new HMACSHA512(); // random hashing provided by .Net which comes with generated key.
            // 'using' automatically dispose class that are stored in memory created in local scope.

            // Update user object values  
            user.UserName = registerDto.UserName.ToLower(); // to compare same object string values due to case sensitivity
            // as passwordhash stores bye arrays, need to get bytes array from computehash
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)); // if do not provide argument or set to null, there will be no reference exception when attempt to get list of bytes
            user.PasswordSalt = hmac.Key;


            _context.Users.Add(user); // simply tracking new entity in memory
            await _context.SaveChangesAsync();// add and save data from tracked users to database.

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs 
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
            var user = await _context.Users
            .Include(p => p.Photos) // load photos to ensure user.Photos not null and check its property if isMain and access the URL
            .SingleOrDefaultAsync(x => 
            x.UserName == loginDto.Username); // 'Find' requires primary key but username is not thus, use either First/SingleOrDefaultAsync

            // Check to see if user exist in data base
            if(user == null) return Unauthorized("invalid username");

            // To get exact match of Hashing algorithm and returns byte array that was used when passed in the same password based on hash key
            // where HMACSHA512 object is created with the user's PasswordSalt value as the key.
            using var hmac = new HMACSHA512(user.PasswordSalt); 

            // Computed Hash version of the from database.
            //The loginDto.Password is hashed using the same algorithm and key as above 
            // Note: it is not possible to obtain the original input data from the computed hash value. 
            // Note: The only way to verify whether a given input data matches the hash value is to apply the same hash function to the input data using the same key, and compare the resulting hash value with the stored hash value.
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Compare password between user and database being stored.
            // The computed hash is compared byte by byte with the PasswordHash value stored in the database,
            // to determine whether the input password provided by the user matches the stored password for the user
            for (int i = 0; i < computedHash.Length; i++) 
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }

            
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs 
            };
            //Overall, this method checks the user's input against the stored user data in the database and returns a token if the input is valid.
        }   

        private async Task<bool>  UserExists(string username)// asynchronous to enter database and check if user has already registered
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower()); // to compare same object string values due to case sensitivity
        }
    }
}