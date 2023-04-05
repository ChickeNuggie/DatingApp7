using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
// An Instance that access database from backend to execute sql command, acts as a bridge connection point between database and backend

    public class DataContext : DbContext
    {
        // [Key] // specify Primary (Unique) Key identification if required
        // Constructor
        //Inject this data contxt into other classess to have access to methods and functionlity of this class
        public DataContext(DbContextOptions options) : base(options) // passing string 'option' to connect to database
        { 
        }

        public DbSet<AppUser> Users { get; set; } // set table name to 'Users' which consist of AppUser class type with ID and UserName
        public DbSet<UserLike> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>() // specify configuration on relationship
            .HasKey(k => new {k.SourceUserId, k.TargetUserId}); // primary key of UserLike table.

            //Lisa likes many other users
            builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade); //delete corresponding likes if user delete profile
        
            //Lisa likes many other users
            builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUser)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade); //delete corresponding likes if user delete profile

        }
    }
}