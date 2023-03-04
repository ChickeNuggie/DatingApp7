using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]//API controller attribute route
    [Route("api/[controller]")]  //API route, GET /api/users (First part of the name of the controller after localhost:5000/ etc.)

    public class BaseApiController : ControllerBase
    {

    }
}