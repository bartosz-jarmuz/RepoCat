using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Portal.Models;

namespace RepoCat.Portal.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult About()
        {
            return this.View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
