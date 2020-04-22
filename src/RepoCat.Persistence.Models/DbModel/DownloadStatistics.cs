// -----------------------------------------------------------------------
//  <copyright file="DownloadStatistics.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
{

    /// <summary>
    /// Statistics of project downloads
    /// </summary>
    public class DownloadStatistics
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [BsonId, BsonRepresentation(BsonType.ObjectId), BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ObjectId RepositoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProjectDownloadData> ProjectDownloadData { get; internal set; } = new List<ProjectDownloadData>();

    }
}