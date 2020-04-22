// -----------------------------------------------------------------------
//  <copyright file="DownloadStatistics.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MongoDB.Bson;

namespace RepoCat.RepositoryManagement.Service
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