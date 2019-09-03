using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Portal.Models;
using RepoCat.Portal.Models.Domain;
using RepoCat.Portal.Services;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    public class SearchController : Controller
    {
        private readonly ManifestsService manifestsService;
        private readonly IMapper mapper;

        public SearchController(ManifestsService manifestsService, IMapper mapper)
        {
            this.manifestsService = manifestsService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel {Repositories = await this.manifestsService.GetRepositories()};

            return this.View(model);
        }

        [HttpGet]
        [Route("search")]
        public async Task<PartialViewResult> Search(string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResult result = await this.manifestsService.FindCurrentProjects(repositoryName, query, isRegex);
            var queryResultViewModel = this.mapper.Map<ManifestQueryResultViewModel>(result);
            queryResultViewModel.QueryString = query;
            queryResultViewModel.IsRegex = isRegex;

            return this.PartialView("_SearchResultPartial", queryResultViewModel);
        }
    }
}
