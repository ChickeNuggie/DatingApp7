using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

         //Async helps to decide what happen next as middleware constantly goes to other middleware with requesting 'next'
         public async Task InvokeAsync(HttpContext context) //access to HTTP context when being passed through middlewares
         {
            try{
                await _next(context); // await next and pass through http context
            }
            //catches exception from application in this middleware
            catch(Exception ex) 
            {
                _logger.LogError(ex, ex.Message); //look at logging system message
                context.Response.ContentType = "application/json"; //need spcify if not inside API controller
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //Check environment and see if running in development mode
                var response = _env.IsDevelopment() // ? indicates optional to avoid causing exception within exception
                // StackTracereturns is a  formatted string representation of this call stack, a string containing the sequence of method calls that led to the exception being thrown.
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response, options);

                // Returns HTTP response that is handled inside this class and not anywhere else in this application.
                await context.Response.WriteAsync(json); 
                

            }
         }
    }
}