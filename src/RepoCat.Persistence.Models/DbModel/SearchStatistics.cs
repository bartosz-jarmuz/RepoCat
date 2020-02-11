// -----------------------------------------------------------------------
//  <copyright file="SearchStatistics.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
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
        [BsonId, BsonRepresentation(BsonType.ObjectId), BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

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
