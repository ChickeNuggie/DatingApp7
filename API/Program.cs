//create web application instance which allows to run app
using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args); 

// Add services (classes) to the container to enable functionality.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration); // extend our extension method on application services
builder.Services.AddIdentityServices(builder.Configuration);

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


app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("https://localhost:4200")); // middleware to map controller endpoints i.e. request comes in mapcontroller direct the request to API endpoint.

app.UseAuthentication(); // Ask if users have valid token
app.UseAuthorization(); // Even if valid, there are rules for authorization  to go to the Authorize endpoint

app.MapControllers();

app.Run(); // command to run application.
