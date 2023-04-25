using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser)
        {
            var query = _context.Users
            .Where(x => x.UserName == username)
            //project onto memberDto and specify configuration to find mapping profiles which it gets
            // from service that added to application service
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            
            // It provides deferred execution and allows for the creation of complex queries that can be translated into SQL or other query languages.
            //avoid retrieving unnecessary data and improve performance.
            //It creates a new IQueryable object that allows for more efficient querying of the underlying data source.
            .AsQueryable();
            //if is current user,ignore query filters where it returns approved photos. (let to use see unapproved own photos)
            if(isCurrentUser) query = query.IgnoreQueryFilters();
            //find the first item in a list of items that meets a certain condition
            //await keyword is used to make sure that the program waits for this method to finish before continuing.
            return await query.FirstOrDefaultAsync(); //first

        } 

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {   
            //Return list of MemberDto
            var query = _context.Users.AsQueryable();
            // build up query based on user parameters
            // (Filter) Exlude current logged in user from the list ofP matches result returned.
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            // Order by last active/creation.
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive) // default in swtich statement
            };

            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                 userParams.PageNumber, 
                 userParams.PageSize);
        }

        // Any tasks returns a value required async
        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await _context.Users
            //Explicity tell entity framework to include and show related data 
            .Include(p => p.Photos)
            .ToListAsync(); 
        }

        public  async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByPhotoId(int photoId)
        {
            return await _context.Users // access user's database
                .Include(p => p.Photos) // show related photos data
                .IgnoreQueryFilters() // ignore query on unapproved photos
                .Where(p => p.Photos.Any(p => p.Id == photoId)) // where any photos id matches the parameter.
                .FirstOrDefaultAsync(); //returns first element.
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            //Explicity tell entity framework to include and show related data 
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username); // x represents AppUser. 
            //It gets full entity from this database and return it in memory to controller
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .Select(x => x.Gender)
                .FirstOrDefaultAsync();
        }

        public void Update(AppUser user)
        {
            // Informs Entity Framework Tracker that an entity 'user' passed in has been updated.
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}