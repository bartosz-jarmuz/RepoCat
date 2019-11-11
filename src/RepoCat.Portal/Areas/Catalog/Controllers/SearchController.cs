using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Models;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    /// <summary>
    /// Handles search requests
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Catalog")]
    public class SearchController : Controller
    {
        private readonly ManifestsService manifestsService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="manifestsService">The manifests service.</param>
        /// <param name="mapper">The mapper.</param>
        public SearchController(ManifestsService manifestsService, IMapper mapper)
        {
            this.manifestsService = manifestsService;
            this.mapper = mapper;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        public async Task<IActionResult> Index()
        {
            SearchIndexViewModel model = new SearchIndexViewModel {Repositories = await manifestsService.GetRepositoryNames().ConfigureAwait(false) };

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
            ManifestQueryResultViewModel queryResultViewModel = await GetQueryResultViewModel(repositoryName, query, isRegex).ConfigureAwait(false);

            return this.PartialView("_SearchResultPartial", queryResultViewModel);
        }

        /// <summary>
        /// Gets the search result page (for URL sharing).
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="query">The query.</param>
        /// <param name="isRegex">if set to <c>true</c> [is regex].</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [Route("{controller}/{repositoryName}/Result")]
        public async Task<IActionResult> GetSearchResultPage(string repositoryName, string query, bool isRegex)
        {
            SearchIndexViewModel model = new SearchIndexViewModel
            {
                Repositories = await manifestsService.GetRepositoryNames().ConfigureAwait(false),
                Repository = repositoryName,
                Query = query,
                IsRegex = isRegex
            };

            ManifestQueryResultViewModel queryResultViewModel = await GetQueryResultViewModel(repositoryName, query, isRegex).ConfigureAwait(false);
            model.Result = queryResultViewModel;
            return this.View("Index", model);
        }

        private async Task<ManifestQueryResultViewModel> GetQueryResultViewModel(string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResult result = await manifestsService.GetCurrentProjects(repositoryName, query, isRegex).ConfigureAwait(false);
            ManifestQueryResultViewModel queryResultViewModel = this.mapper.Map<ManifestQueryResultViewModel>(result);
            return queryResultViewModel;
        }
    }
}
