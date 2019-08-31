using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManifestsService manifestsService;

        public HomeController(ManifestsService manifestsService)
        {
            this.manifestsService = manifestsService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel {Repositories = await this.manifestsService.GetRepositories()};

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
