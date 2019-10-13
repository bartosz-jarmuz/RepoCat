using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepoCat.Persistence.Service;

namespace RepoCat.Portal.Areas.Catalog.Views.Shared.Components.RepositoryList
{
    /// <summary>
    /// Class RepositoryList.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ViewComponent" />
    public class RepositoryList : ViewComponent
    {
        /// <summary>
        /// The service
        /// </summary>
        private readonly ManifestsService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryList"/> class.
        /// </summary>
        /// <param name="manifestsService">The manifests service.</param>
        public RepositoryList(ManifestsService manifestsService)
        {
            this.service = manifestsService;
        }

        /// <summary>
        /// invoke as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;IViewComponentResult&gt;.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var names = await this.service.GetRepositoryNames();
            var model = new RepositoriesListViewModel {Repositories = names};
            return this.View("~/Areas/Catalog/Views/Shared/Components/RepositoryList/Default.cshtml", model);
        }
    }
}