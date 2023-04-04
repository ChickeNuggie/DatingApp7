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

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //project onto memberDto and specify configuration to find mapping profiles which it gets
            // from service that added to application service
            .SingleOrDefaultAsync();
            
        } 

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {   
            //Return list of MemberDto
            var query= _context.Users.AsQueryable();
            // build up query based on user parameters
            // (Filter) Exlude current logged in user from the list of matches result returned.
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

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            //Explicity tell entity framework to include and show related data 
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username); // x represents AppUser. 
            //It gets full entity from this database and return it in memory to controller
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // To indicate if changes saved in database, 0 = false
        }

        public void Update(AppUser user)
        {
            // Informs Entity Framework Tracker that an entity 'user' passed in has been updated.
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}