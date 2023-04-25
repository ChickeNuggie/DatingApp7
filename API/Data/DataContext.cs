using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace API.Data
{
// An Instance that access database from backend to execute sql command, acts as a bridge connection point between database and backend
// Note: abstraction of the database.
    //need to specify the class that we have created(appuser, approle, claims, etc) and specify int values.
    //Note: order of classess is important!!
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        // [Key] // specify Primary (Unique) Key identification if required
        // Constructor
        //Inject this data contxt into other classess to have access to methods and functionlity of this class
        public DataContext(DbContextOptions options) : base(options) // passing string 'option' to connect to database
        { 
        }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Photo> Photos { get; set; }

        //Configure database context. (tables)
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey( ur => ur.UserId)
                .IsRequired(); // foreign key is required.

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey( ur => ur.RoleId)
                .IsRequired(); // foreign key is required.


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

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict); // prevent deletion from sender to allow recipient to view messages
        
            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict); // prevent deletion from sender to allow recipient to view messages

            //Query filter to only return approved photos.
            //This means that the filter will be applied to all queries that retrieve instances of the Photo entity.
            // only photos that have the IsApproved property set to true should be included in the results of any query that retrieves instances of the Photo entity.
            // ensure that only approved photos are ever retrieved from the database, without having to manually add the IsApproved condition to every query that retrieves photos.
            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
            
            // builder.ApplyUtcDateTimeConverter();
            
        }
    }
}