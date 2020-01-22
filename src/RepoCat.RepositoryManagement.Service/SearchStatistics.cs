// -----------------------------------------------------------------------
//  <copyright file="SearchStatistics.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchStatistics
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SearchKeywordData> SearchKeywordData { get; internal set; } = new List<SearchKeywordData>();

    }
}