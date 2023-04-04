using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //Type of action filter
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]//API controller attribute route
    [Route("api/[controller]")]  //API route, GET /api/users (First part of the name of the controller after localhost:5000/ etc.)

    public class BaseApiController : ControllerBase
    {

    }
}