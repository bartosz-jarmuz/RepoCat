// -----------------------------------------------------------------------
//  <copyright file="RepositoryList.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Views.Components
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
        private readonly IRepositoryManagementService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryList"/> class.
        /// </summary>
        /// <param name="repositoryDatabase">The manifests service.</param>
        public RepositoryList(IRepositoryManagementService repositoryDatabase)
        {
            this.service = repositoryDatabase;
        }

        /// <summary>
        /// invoke as an asynchronous operation.
        /// </summary>
        /// <returns>Task&lt;IViewComponentResult&gt;.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var repositoryGroups = await this.service.GetAllRepositoriesGrouped().ConfigureAwait(false);

            
            var model = new RepositoriesListViewModel();
            foreach (var repositoryGroup in repositoryGroups)
            {
                model.Repositories.Add(new OrganizationRepositoryGroup()
                {
                    OrganizationName =  repositoryGroup.OrganizationName,
                    Repositories = repositoryGroup.Repositories.Select(x=>x.RepositoryName).ToList()
                });
            }


            return this.View("~/Areas/Catalog/Views/Shared/Components/RepositoryList/Default.cshtml", model);
        }
    }
}