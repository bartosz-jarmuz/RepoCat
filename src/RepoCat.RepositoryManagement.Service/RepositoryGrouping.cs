// -----------------------------------------------------------------------
//  <copyright file="RepositoryGrouping.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// Represents a collection of repositories from a single organization
    /// </summary>
    public class RepositoryGrouping
    {
        /// <summary>
        /// Creates new instance
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositories"></param>
        public RepositoryGrouping(string organizationName, IEnumerable<RepositoryInfo> repositories)
        {
            this.OrganizationName = organizationName;
            this.Repositories = repositories.ToList();
        }

        /// <summary>
        /// Creates the collection of groupings
        /// </summary>
        /// <param name="repositories"></param>
        /// <returns></returns>
        public static IEnumerable<RepositoryGrouping> CreateGroupings(IEnumerable<RepositoryInfo> repositories)
        {
            return repositories.GroupBy(x => x.OrganizationName).Select(x => new RepositoryGrouping(x.Key, x.ToList()));
        }

        /// <summary>
        /// Name of the organization that all repositories belong to
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Collection of repositories
        /// </summary>
        public ICollection<RepositoryInfo> Repositories { get; }
    }
}