using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RepoCat.Portal.Models;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ManifestsService manifestsService;
        private readonly IMapper mapper;

        public HomeController(ManifestsService manifestsService, IMapper mapper)
        {
            this.manifestsService = manifestsService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel {Repositories = await this.manifestsService.GetRepositories()};

            return View(model);
        }

        [HttpGet]
        [Route("search")]
        public async Task<PartialViewResult> Search(string repositoryName, string query)
        {
            var result = await this.manifestsService.FindCurrentProjects(repositoryName, query);
            List<ProjectManifestViewModel> manifests = this.mapper.Map<List<ProjectManifestViewModel>>(result.Manifests);

            return this.PartialView("_ProjectsListPartial", manifests);
        }

        public IActionResult About()
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
