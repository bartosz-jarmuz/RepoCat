// -----------------------------------------------------------------------
//  <copyright file="OrganizationRepositoryGroup.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.Portal.Areas.Catalog.Views.Components
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
        public List<string> Repositories { get; internal set; } = new List<string>();

    }
}