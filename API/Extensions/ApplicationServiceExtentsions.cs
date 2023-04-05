
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{   // Add services that allows to use any related database access and outside of the database access in several different parts of the application.
    // make class static to use method inside class without instantiate a new instance of this class
    public static class ApplicationServiceExtentsions
    {
        // extends IServiceCollection
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            //In Summary, the below code configures the application to use a DataContext instance for database access and specifies that the data should be stored in an SQLite database using the connection string named "DefaultConnection".
            services.AddDbContext<DataContext>(opt =>  //specify method 'opt' being passed in.
            //The lambda expression is used to configure the DbContextOptions instance that will be used to create the DataContext instance.
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
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
            services.AddCors();

            //Singletonis best used when caching service and you want every single request that comes to your API, save response from request into cache to be retrieved data next time. (May waste memory)
            //Transient created and disposed of when its used. Short-lived for HTTP request service.
            //Scoped: when controllers are disposed of at the end of HTTP request, any dependent services are also disposed.
            services.AddScoped<ITokenService, TokenService>(); // have to provide both interface and its implemented class
            // able to isolate test without implementing other context.

            // makes this classes injectable to user controller
            services.AddScoped<IUserRepository, UserRepository>(); 

            //Add automapper service on assembly on current domain (single project)
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add configuration Cloundinary settings class to app service extension that matches exactly from appsettings.son
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            // Add services and interface of PhotoService into application service extension in order to function and inject to other classes.
            services.AddScoped<IPhotoService, PhotoService>();
            
            //Add services to update user log activity into the application
            services.AddScoped<LogUserActivity>();

            //Add services to access likes repository
            services.AddScoped<ILikesRepository, LikesRepository>();
            return services;
        }
    }
}