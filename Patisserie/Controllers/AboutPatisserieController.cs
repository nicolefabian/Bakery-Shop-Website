using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Patisserie.Controllers
{

    //allow anyone to view the about us page
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
