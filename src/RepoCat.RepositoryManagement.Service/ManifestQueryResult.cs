﻿using System;
using System.Collections.Generic;
using System.Linq;
using RepoCat.Persistence.Models;

namespace RepoCat.RepositoryManagement.Service
{
    /// <summary>
    /// Encapsulates the result of querying the database
    /// </summary>
    public class ManifestQueryResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repoParams"></param>
        /// <param name="projects"></param>
        /// <param name="elapsed"></param>
        /// <param name="queryString"></param>
        /// <param name="isRegex"></param>
        public ManifestQueryResult(IEnumerable<RepositoryQueryParameter> repoParams, IEnumerable<Project> projects,
            TimeSpan elapsed, string queryString, bool isRegex)
        {
            this.RepositoryQueryParameters = repoParams;
            this.Elapsed = elapsed;
            this.IsRegex = isRegex;
            this.QueryString = queryString;
            this.Projects = projects.ToList();
        }

        /// <summary>
        /// Gets or sets the project infos.
        /// </summary>
        /// <value>The manifests.</value>
        public IReadOnlyList<Project> Projects { get; internal set; }

        public IEnumerable<RepositoryQueryParameter> RepositoryQueryParameters { get; set; }

        /// <summary>
        /// How long it took to execute the query
        /// </summary>
        public TimeSpan Elapsed { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this search was regex.
        /// </summary>
        /// <value><c>true</c> if this instance is regex; otherwise, <c>false</c>.</value>
        public bool IsRegex { get; set; }
        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString { get; set; }

    }
}