using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Patisserie.Controllers
{
    //[Authorize(Roles = "Administrator")]

    //allow anyone
    [AllowAnonymous]
    public class AboutPatisserieController : Controller
    {
        //display about the bakery shop 
        public IActionResult Index()
        {
            return View();
        }
    }
}
