using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions // static as creating extension method that allows uses of all methods from this extension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value; // uniquename 
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value); // uniquename 
        }
    }
}