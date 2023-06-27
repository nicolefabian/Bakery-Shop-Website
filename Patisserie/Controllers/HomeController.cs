using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Patisserie.Models;
using Patisserie.Models.DB;
using System.Diagnostics;
using X.PagedList;

namespace Patisserie.Controllers
{
    [AllowAnonymous] //allow anyone to view homepage
    public class HomeController : Controller
    {
        private readonly FSWD2023fabi18Context _context;

        public HomeController(FSWD2023fabi18Context context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}