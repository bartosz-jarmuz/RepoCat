// -----------------------------------------------------------------------
//  <copyright file="RepoCatDbSettings.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Persistence.Service
{
    /// <inheritdoc />
    public class RepoCatDbSettings : IRepoCatDbSettings
    {
        /// <inheritdoc />
        public string DownloadsStatisticsCollectionName { get; set; }
        /// <inheritdoc />
        public string SearchStatisticsCollectionName { get; set; }
        /// <inheritdoc />
        public string RepositoriesCollectionName { get; set; }
        /// <inheritdoc />
        public string ProjectsCollectionName { get; set; }
        /// <inheritdoc />
        public string ConnectionString { get; set; }
        /// <inheritdoc />
        public string DatabaseName { get; set; }
    }
}
