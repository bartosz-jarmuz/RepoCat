using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RepoCat.RepositoryManagement.Service;

namespace RepoCat.Portal.Areas.Catalog.Models
{
    /// <summary>
    /// Class SearchIndexViewModel.
    /// </summary>
    public class SearchIndexViewModel
    {
        /// <summary>
        /// Gets or sets the repository.
        /// </summary>
        /// <value>The repository.</value>
        public string Repository { get; set; }
        /// <summary>
        /// Gets or sets the repositories.
        /// </summary>
        /// <value>The repositories.</value>
        public List<SelectListItem> Repositories { get; internal set; } 
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public ManifestQueryResultViewModel Result { get; set; }
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public string Query { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is regex.
        /// </summary>
        /// <value><c>true</c> if this instance is regex; otherwise, <c>false</c>.</value>
        public bool IsRegex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SearchKeywordData> TopSearchedTags { get; set; } = new List<SearchKeywordData>();
    }

}
