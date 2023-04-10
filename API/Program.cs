//create web application instance which allows to run app.
// main entry point of the program
//It contains the code to instantiate and run the application, as well as any initialization or configuration code that needs to be executed before the application starts. 
// Used when whenever you need to define the entry point of your C# application and perform any necessary initialization or configuration.
using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); 

// Add services (classes) to the container to enable functionality.
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration); // extend our extension method on application services
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Middleware:
// Configure the HTTP request pipeline. (request comes into API i.e. weartherforecast endpoint)
// Able to edit the request sent into HTTP via adding code/services i.e. Authorization
// if (app.Environment.IsDevelopment()) // check if is in development mode
// {
//     // use swagger and its interface
//     app.UseSwagger(); 
//     app.UseSwaggerUI();
// }

// app.UseAuthorization();

// BException Handling app before http request pipeline
app.UseMiddleware<ExceptionMiddleware> ();

app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200")); // middleware to map controller endpoints i.e. request comes in mapcontroller direct the request to API endpoint.

//Authentication middleware: it tells server how to authenticate the Jwt tokens???. (where order is important)
app.UseAuthentication(); // Ask if users have valid token
app.UseAuthorization(); // Even if valid, there are rules for authorization  to go to the Authorize endpoint

app.MapControllers();

// allow access to all services included inside program class.
// To automatically apply Seed class to data in application
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try 
{   //To handle exception as it is not an http request which does not go through an http request pipeline.
    //get data from user's database using required service,
    var context = services.GetRequiredService<DataContext>();
    // usermanager service in order to pass seedusers method.
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    // rolemanager service in order to pass seedusers method.
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    // Just dropdatabase and restart API to reset everything (clena slate data).
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager); 
}
catch (Exception ex) 
{   //get Logger service and throw exception error.
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");

}
app.Run(); // command to run application.
