// -----------------------------------------------------------------------
//  <copyright file="ManifestQueryResultViewModel.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Utilities;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Contains the result of a query
    /// </summary>
    public class ManifestQueryResultViewModel
    {
        /// <summary>
        /// The search tokens
        /// </summary>
        private List<string> searchTokens;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is regex.
        /// </summary>
        /// <value><c>true</c> if this instance is regex; otherwise, <c>false</c>.</value>
        public bool IsRegex { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>The query string.</value>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets the search tokens.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        private List<string> GetSearchTokens()
        {
            if (!this.IsRegex)
            {
                return QueryStringTokenizer.GetTokens(this.QueryString);
            }
            else
            {
               return new List<string>() {this.QueryString };
            }

        }

        /// <summary>
        /// Gets or sets the search tokens.
        /// </summary>
        /// <value>The search tokens.</value>
        public List<string> SearchTokens
        {
            get
            {
                if (this.searchTokens == null)
                {
                    this.searchTokens = this.GetSearchTokens();
                }
                return this.searchTokens;
            }
            internal set => this.searchTokens = value;
        }

        /// <summary>
        /// Projects table
        /// </summary>
public ProjectsTableModel ProjectsTable { get; set; }
        /// <summary>
        /// Gets or sets the elapsed.
        /// </summary>
        /// <value>The elapsed.</value>
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<RepositoryQueryParameter> RepositoryQueryParameters { get; set; }

     
    }
}