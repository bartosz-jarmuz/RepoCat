using System;
using System.Collections.Generic;

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// Encapsulates the result of querying the database
    /// </summary>
    public class ManifestQueryResult
    {
        /// <summary>
        /// Gets or sets the project infos.
        /// </summary>
        /// <value>The manifests.</value>
        public List<Project> Projects { get;  set; } = new List<Project>();
        /// <summary>
        /// How long it took to execute the query
        /// </summary>
        public TimeSpan Elapsed { get; set; }
        /// <summary>
        /// The stamp of the repository from which the result comes
        /// </summary>
        public string RepositoryStamp { get; set; }
        /// <summary>
        /// Gets or sets the name of the queried repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        public string RepositoryName { get; set; }
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

        /// <summary>
        /// Name of the organization in which repo is
        /// </summary>
        public string OrganizationName { get; set; }
    }
}