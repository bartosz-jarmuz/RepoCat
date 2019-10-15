using System;
using System.Collections.Generic;
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
            set => this.searchTokens = value;
        }

        /// <summary>
        /// Gets or sets the manifests.
        /// </summary>
        /// <value>The manifests.</value>
        public List<ProjectInfoViewModel> ProjectInfos { get; set; } = new List<ProjectInfoViewModel>();
        /// <summary>
        /// Gets or sets the elapsed.
        /// </summary>
        /// <value>The elapsed.</value>
        public TimeSpan Elapsed { get; set; }
        /// <summary>
        /// Gets or sets the repo stamp.
        /// </summary>
        /// <value>The repo stamp.</value>
        public string RepositoryStamp { get; set; }
        /// <summary>
        /// Gets or sets the repo.
        /// </summary>
        /// <value>The repo.</value>
        public string RepositoryName { get; set; }
    }
}