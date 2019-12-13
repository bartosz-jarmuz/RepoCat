using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Views.Shared.Components.RepositoryList
{
    /// <summary>
    /// List of repositories within a given organization
    /// </summary>
    public class OrganizationRepositoryGroup
    {
        /// <summary>
        /// Name of the organization
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Repositories
        /// </summary>
        public List<string> Repositories { get; set; } = new List<string>();

    }
}