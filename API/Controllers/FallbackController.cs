using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{// need access to a view and effectively navigate to physical .html file if it does not know what to do with particular route.
    public class FallbackController : Controller
    {
        public ActionResult Index()
        {//go into current directory(api folder) and direct to wwwroot and get the index.html which is type of text/HTML.
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
                 "wwwroot", "index.html"), "text/HTML");
        }
    }
}