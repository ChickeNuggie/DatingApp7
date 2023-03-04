using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
         // specify this serice that we are extending and its configuration.
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration config)
        {
        // service and middlware where it tells server how to authenticate the Jwt tokens.
        // As request comes in, the request can be inspected and then framework can decide if user can proceed based on their authentication token.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                { // specify rules on how the server should validate as good token

                    //Server checks the token signing key and make sure is valid based upon the issuer signing key (avoid accepting random token signing key)
                    ValidateIssuerSigningKey = true,
                    //specify what the issuer signing key is and get its array of bytes.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .UTF8.GetBytes(config["TokenKey"])), // exact same key used in token service.
                    ValidateIssuer = false, //API server does not have information to pass information down with token in order to validate it
                    ValidateAudience = false // info not passed with token yet.
                };
            });
            return services;
        }
    }
}