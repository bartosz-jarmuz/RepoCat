using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Views.Shared.Components.RepositoryList
{
    /// <summary>
    /// Class RepositoriesListViewModel.
    /// </summary>
    public class RepositoriesListViewModel
    {
        /// <summary>
        /// Gets or sets the repositories.
        /// </summary>
        /// <value>The repositories.</value>
        public List<OrganizationRepositoryGroup> Repositories { get;  } = new List<OrganizationRepositoryGroup>();
    }
}
