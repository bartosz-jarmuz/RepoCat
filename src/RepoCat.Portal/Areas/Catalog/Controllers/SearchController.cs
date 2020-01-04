using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Areas.Catalog.Models;
using RepoCat.Portal.Models;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Controllers
{
    /// <summary>
    /// Handles search requests
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Area("Catalog")]
    public class SearchController : Controller
    {
        private readonly RepositoryDatabase repositoryDatabase;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="repositoryDatabase">The manifests service.</param>
        /// <param name="mapper">The mapper.</param>
        public SearchController(RepositoryDatabase repositoryDatabase, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.repositoryDatabase = repositoryDatabase;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        public async Task<IActionResult> Index()
        {
            SearchIndexViewModel model = new SearchIndexViewModel();
            model.Repositories = await this.GetRepositoriesSelectList().ConfigureAwait(false);

            return this.View(model);
        }

        private async Task<List<SelectListItem>> GetRepositoriesSelectList()
        {
            IReadOnlyCollection<RepositoryGrouping> groups = await this.repositoryDatabase.GetAllRepositoriesGrouped().ConfigureAwait(false);

            List<SelectListItem> items = new List<SelectListItem>();

            foreach (RepositoryGrouping repoGroup in groups)
            {
                SelectListGroup group = new SelectListGroup() { Name = repoGroup.OrganizationName };
                foreach (RepositoryInfo repo in repoGroup.Repositories)
                {
                    SelectListItem item = new SelectListItem() { Text = repo.RepositoryName, Value = $"{repo.OrganizationName}:{repo.RepositoryName}", Group = group };
                    items.Add(item);
                }

            }
            return items;
        }


        /// <summary>
        /// Search the repository for a specified query
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Search/{organizationName}/{repositoryName}")]
        public async Task<PartialViewResult> Search(string organizationName, string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(organizationName, repositoryName, query, isRegex).ConfigureAwait(false);
            this.telemetryClient.TrackSearch(organizationName, repositoryName, query, isRegex);
            return this.PartialView("_SearchResultPartial", queryResultViewModel);
        }

        /// <summary>
        /// Gets the search result page (for URL sharing).
        /// </summary>
        /// <param name="organizationName">Name of the organization which holds the repository</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="query">The query.</param>
        /// <param name="isRegex">if set to <c>true</c> [is regex].</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [Route("{controller}/{organizationName}/{repositoryName}/Result")]
        public async Task<IActionResult> GetSearchResultPage(string organizationName, string repositoryName, string query, bool isRegex)
        {
            SearchIndexViewModel model = new SearchIndexViewModel
            {
                Repositories = await this.GetRepositoriesSelectList().ConfigureAwait(false),
                Repository = repositoryName,
                Query = query,
                IsRegex = isRegex
            };

            ManifestQueryResultViewModel queryResultViewModel = await this.GetQueryResultViewModel(organizationName, repositoryName, query, isRegex).ConfigureAwait(false);
            model.Result = queryResultViewModel;
            return this.View("Index", model);
        }

        private async Task<ManifestQueryResultViewModel> GetQueryResultViewModel(string organizationName, string repositoryName, string query, bool isRegex)
        {
            ManifestQueryResult result = await this.repositoryDatabase.GetCurrentProjects(organizationName, repositoryName, query, isRegex).ConfigureAwait(false);
            ManifestQueryResultViewModel queryResultViewModel = this.mapper.Map<ManifestQueryResultViewModel>(result);
            return queryResultViewModel;
        }
    }
}
