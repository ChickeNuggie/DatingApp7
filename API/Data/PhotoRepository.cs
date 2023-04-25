using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        
        public PhotoRepository(DataContext context)
        {
            _context = context;
            
        }

        //asynchronously, meaning it can perform tasks in the background while the program continues to execute other tasks.
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(x => x.Id == id); //used to execute the query and return a single object that matches the condition.
        }

        //returns a Task that contains a collection of "PhotoForApprovalDto" objects.
        //this method can run asynchronously, meaning it can perform tasks in the background while the program continues to execute other tasks.
        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos // retrieves all photos by calling the ".Photos" property of the context.
                .IgnoreQueryFilters() //retrieve all photos regardless of any filters applied.
                .Where(p => p.IsApproved == false) //  only retrieve those where the "IsApproved" property is false, meaning they have not been approved yet.
                .Select(u => new PhotoForApprovalDto //transform each photo object into a new "PhotoForApprovalDto" object.(prevent referencing trap)
                {
                    Id = u.Id,
                    UserName = u.AppUser.UserName,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).ToListAsync(); //to execute the query asynchronously and retrieve the results as a list
                
        }

        public void RemovedPhoto(Photo photo)
        {   //access database with 'photo' table and remove the photo passed in parameter.
            _context.Photos.Remove(photo);
        }
    }
}