using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Models;

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
            SearchIndexViewModel model = new SearchIndexViewModel {Repositories = await this.manifestsService.GetRepositories()};

            return this.View(model);
        }


        /// <summary>
        /// Search the repository for a specified query
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search/{repositoryName}")]
        public async Task<PartialViewResult> Search(string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(repositoryName, query, isRegex);

            return this.PartialView("_SearchResultPartial", queryResultViewModel);
        }

        [HttpGet]
        [Route("{controller}/{repositoryName}/Result")]
        public async Task<IActionResult> GetSearchResultPage(string repositoryName, string query, bool isRegex)
        {
            SearchIndexViewModel model = new SearchIndexViewModel
            {
                Repositories = await this.manifestsService.GetRepositories(),
                Repository = repositoryName,
                Query = query,
                IsRegex = isRegex
            };

            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(repositoryName, query, isRegex);
            model.Result = queryResultViewModel;
            return this.View("Index", model);
        }

        private async Task<ManifestQueryResultViewModel> GetQueryResultViewModel(string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResult result = await this.manifestsService.GetCurrentProjects(repositoryName, query, isRegex);
            ManifestQueryResultViewModel queryResultViewModel = this.mapper.Map<ManifestQueryResultViewModel>(result);
            return queryResultViewModel;
        }
    }
}
