using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed 
    {   // access class method instantly 'Seed.SeedUser' without creating new instance 'var seed = new Seed();'
        public static async Task SeedUsers(DataContext context) 
        {
            //check if user exist in database to prevent seeding same user.
            if (await context.Users.AnyAsync()) return; //stop execution of this method.

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            //Case sensitive when retrieve data.
            var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

            //convert Json to C# objects.
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            //Generate password for each users.
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower(); // for comparing users to users to be consistent
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")); // complex password set to test functionality of seed users
                user.PasswordSalt = hmac.Key;

                //Adding user inside each loop            
                context.Users.Add(user); // add to entity framework tracking
            }
            // save changes to users to database
            await context.SaveChangesAsync();

            //Note: To automatically apply seed to data in application, best to apply when application 'Program.cs' start up (entry point of application)  
            //(without using .Net SQL tool 'dotnet ef mirgrations/add/update/remove')
         }
    }
}