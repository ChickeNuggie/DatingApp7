//create web application instance which allows to run app
using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); 

// Add services (classes) to the container to enable functionality.

builder.Services.AddControllers();

//In Summary, the below code configures the application to use a DataContext instance for database access and specifies that the data should be stored in an SQLite database using the connection string named "DefaultConnection".
builder.Services.AddDbContext<DataContext>(opt =>  //specify method 'opt' being passed in.
//The lambda expression is used to configure the DbContextOptions instance that will be used to create the DataContext instance.
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // configures the DbContextOptions to use SQLite as the underlying database provider.
    // retrieves the connection string named "DefaultConnection" from the application's configuration. 
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

//Cross-Origin Resource Sharing which is a security mechanism that is enforced by web browsers to protect against unauthorized cross-domain requests.
// ability to make cross-domain requests from an Angular application to a server that is located on a different domain.
// By default, web browsers block cross-origin requests initiated by scripts for security reasons. 
// The server needs to include the appropriate CORS headers in its response to tell web browser that it is safe to allow angular app to access resources on server from different domain..
builder.Services.AddCors();

var app = builder.Build();

// Middleware:
// Configure the HTTP request pipeline. (request comes into API i.e. weartherforecast endpoint)
// Able to edit the request sent into HTTP via adding code/services i.e. Autorhization
// if (app.Environment.IsDevelopment()) // check if is in development mode
// {
//     // use swagger and its interface
//     app.UseSwagger(); 
//     app.UseSwaggerUI();
// }

// app.UseAuthorization();


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
 // middleware to map controller endpoints i.e. request comes in mapcontroller direct the request to API endpoint.
app.MapControllers();

app.Run(); // command to run application.
