using API.DTO;
using API.Entities;
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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {   
            //Return list of MemberDto
            return await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) //Note: caching is faster than mapping users to DTO to retrieve relevant information
            .ToListAsync();
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