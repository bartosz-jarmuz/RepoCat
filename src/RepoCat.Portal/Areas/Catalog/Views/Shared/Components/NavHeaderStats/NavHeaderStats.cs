using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;
using RepoCat.RepositoryManagement.Service;
using RepositoryQueryParameter = RepoCat.RepositoryManagement.Service.RepositoryQueryParameter;

namespace RepoCat.Portal.Areas.Catalog.Views.Components
{
    /// <summary>
    /// Class NavHeaderStats.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ViewComponent" />
    public class NavHeaderStats : ViewComponent
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly IRepositoryManagementService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryList"/> class.
        /// </summary>
        /// <param name="repositoryService">The manifests service.</param>
        public NavHeaderStats(IRepositoryManagementService repositoryService)
        {
            this.service = repositoryService;
        }

        /// <summary>
        /// invoke as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;IViewComponentResult&gt;.</returns>
        ///
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IReadOnlyCollection<RepositoryGrouping> repositoryGroups = await this.service.GetAllRepositoriesGrouped().ConfigureAwait(false);

            List<RepositoryQueryParameter> parameters = repositoryGroups.SelectMany(g => g.Repositories.Select(r => new RepositoryQueryParameter(r))).ToList();
            ManifestQueryResult result = await this.service.GetCurrentProjects(parameters, "", false).ConfigureAwait(false);
            List<ComponentManifest> components = result.Projects.SelectMany(x => x.ProjectInfo.Components).ToList();
            int tags = components.Sum(x => x.Tags.Count);
            int props = components.Sum(x => x.Properties.Count);

            NavHeaderStatsViewModel model = new NavHeaderStatsViewModel()
            {
                RepositoriesCount = repositoryGroups.SelectMany(x => x.Repositories).Count(),
                OrganizationsCount = repositoryGroups.Count,
                ProjectsCount = result.Projects.Count,
                ComponentsCount = components.Count,
                PropertiesCount = props,
                TagsCount = tags
            };

            return this.View("~/Areas/Catalog/Views/Shared/Components/NavHeaderStats/Default.cshtml", model);
        }
    }
}