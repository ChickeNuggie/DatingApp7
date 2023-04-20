using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed 
    {   
        public static async Task ClearConnections(DataContext context)
        {
            context.Connections.RemoveRange(context.Connections);
            await context.SaveChangesAsync();
        }
        
        
        // access class method instantly 'Seed.SeedUser' without creating new instance 'var seed = new Seed();'
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager) 
        {
            //check if user exist in database to prevent seeding same user.
            if (await userManager.Users.AnyAsync()) return; //stop execution of this method.

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //Case sensitive when retrieve data.
            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            //convert Json to C# objects.
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            //create roles for the application
            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin" },
                new AppRole{Name = "Moderator" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            //Generate password for each users.
            foreach (var user in users)
            {

                user.UserName = user.UserName.ToLower(); // for comparing users to users to be consistent
                //specify UTC date time in postgres database
                user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
                user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
                //It will create and save changes in database
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

            }

            //Note: To automatically apply seed to data in application, best to apply when application 'Program.cs' start up (entry point of application)  
            //(without using .Net SQL tool 'dotnet ef mirgrations/add/update/remove')

            var admin = new AppUser{
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            // add user to multiple multiple roles
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}); 
        
         }
    }
}