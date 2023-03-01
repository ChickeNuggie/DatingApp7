using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        // [Key] // specify Primary (Unique) Key identification if required
        // Constructor
        //Inject this data contxt into other classess to have access to methods and functionlity of this class
        public DataContext(DbContextOptions options) : base(options) // passing string 'option' to connect to database
        { 
        }

        public DbSet<AppUser> Users { get; set; } // set table name to 'Users' which consist of AppUser class type with ID and UserName
    }
}