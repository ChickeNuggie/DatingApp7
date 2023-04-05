using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        // do something before or after next API action or endpoint (has it executed its functionality?)       
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //returns result context after API action is completed and executed
            // Wait API finished its task (i.e. use logged in) and update last activity property inside the user.
            var resultContext = await next();

            // not necessarily to implement to authenticate user as it has been authenticated upon loggin
            // HttpConext allows access to get hold of users' username and services as well to update this particular property.
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; 

            var userId = resultContext.HttpContext.User.GetUserId();

            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            //update one property for user obtained from repository.
            var user = await repo.GetUserByIdAsync(userId); 
            user.LastActive = DateTime.UtcNow;
            await repo.SaveAllAsync();// update database
        }
    }
}