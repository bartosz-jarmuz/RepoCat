using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
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
        public ICollection<RepositoryInfo> Repositories { get;  }
    }

    /// <summary>
    /// Info about repository
    /// </summary>
    public class RepositoryInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [BsonId, BsonRepresentation(BsonType.ObjectId), BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Name of the repository
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// Name of the organization owning the repository
        /// </summary>
        public string OrganizationName { get; set; }

        /// <summary>
        /// Mode of project storage in the repository
        /// </summary>
        public RepositoryMode RepositoryMode { get; set; }
    }
}
