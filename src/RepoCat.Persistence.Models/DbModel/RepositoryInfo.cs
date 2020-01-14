using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
{
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
