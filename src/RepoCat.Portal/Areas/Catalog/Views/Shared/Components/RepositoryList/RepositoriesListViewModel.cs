// -----------------------------------------------------------------------
//  <copyright file="RepositoriesListViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Views.Components
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
