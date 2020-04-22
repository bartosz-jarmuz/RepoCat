// -----------------------------------------------------------------------
//  <copyright file="StatisticsDatabase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RepoCat.Persistence.Models;

namespace RepoCat.Persistence.Service
{
    /// <summary>
    /// Allows access to the stored Manifests data
    /// </summary>
    public class StatisticsDatabase
    {
        private readonly IMongoCollection<SearchStatistics> searchStatisticsCollection;
        private readonly IMongoCollection<DownloadStatistics> downloadsStatisticsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDatabase"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public StatisticsDatabase(IRepoCatDbSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.searchStatisticsCollection = database.GetCollection<SearchStatistics>(settings.SearchStatisticsCollectionName ?? nameof(SearchStatistics));
            this.downloadsStatisticsCollection = database.GetCollection<DownloadStatistics>(settings.DownloadsStatisticsCollectionName??nameof(DownloadStatistics));
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            this.searchStatisticsCollection.Indexes.CreateOne(
                new CreateIndexModel<SearchStatistics>(
                    Builders<SearchStatistics>.IndexKeys
                        .Ascending(r => r.OrganizationName)
                        .Ascending(r => r.RepositoryName), new CreateIndexOptions()
                    {
                        Sparse = true,
                        Unique = true
                    }
                )
            );
        }

        /// <summary>
        /// Updates the search statistics 
        /// </summary>
        /// <param name="repositoryParameter"></param>
        /// <returns></returns>
        public async Task<SearchStatistics> Get(RepositoryQueryParameter repositoryParameter)
        {
            void CheckParams()
            {
                if (repositoryParameter == null) throw new ArgumentNullException(nameof(repositoryParameter));
            }

            CheckParams();

            FilterDefinition<SearchStatistics> repoNameFilter =
                RepoCatFilterBuilder.BuildStatisticsRepositoryFilter(repositoryParameter.OrganizationName,
                    repositoryParameter.RepositoryName);

            return await this.FindOneOrCreateNewAsync(repositoryParameter, repoNameFilter);
        }

        /// <summary>
        /// Gets all the stats
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SearchStatistics>> Get()
        {
            return await (await this.searchStatisticsCollection.FindAsync(FilterDefinition<SearchStatistics>.Empty)
                .ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
        }



        /// <summary>
        /// Updates the search statistics 
        /// </summary>
        /// <param name="repositoryParameter"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public async Task<SearchStatistics> Update(RepositoryQueryParameter repositoryParameter, IEnumerable<string> keywords)
        {
            void CheckParams()
            {
                if (repositoryParameter == null) throw new ArgumentNullException(nameof(repositoryParameter));
            }

            CheckParams();

            FilterDefinition<SearchStatistics> repoNameFilter =
                RepoCatFilterBuilder.BuildStatisticsRepositoryFilter(repositoryParameter.OrganizationName,
                    repositoryParameter.RepositoryName);

            var statistics = await this.FindOneOrCreateNewAsync(repositoryParameter, repoNameFilter);

            foreach (string keyword in keywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }
                var existing = statistics.SearchKeywordData.FirstOrDefault(x =>
                    string.Equals(x.Keyword, keyword, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    existing.SearchCount++;
                }
                else
                {
                    statistics.SearchKeywordData.Add(new SearchKeywordData()
                    {
                        Keyword = keyword, 
                        SearchCount = 1
                    });
                }
            }

            await this.searchStatisticsCollection.ReplaceOneAsync(repoNameFilter, statistics);

            return statistics;
        }

        

        /// <summary>
        /// Increments the number of times a project was downloaded
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<DownloadStatistics> UpdateProjectDownloads(ProjectInfo project)
        {
            FilterDefinition<DownloadStatistics> repositoryFilter = RepoCatFilterBuilder.BuildStatisticsRepositoryFilter(project.RepositoryId);

            var statistics = await this.FindOneOrCreateNewAsync(project.RepositoryId, repositoryFilter);

            ProjectDownloadData existing = statistics.ProjectDownloadData.FirstOrDefault(x =>
                x.ProjectKey.Equals(project.ProjectUri, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.DownloadCount++;
            }
            else
            {
                statistics.ProjectDownloadData.Add(new ProjectDownloadData()
                {
                    ProjectKey = project.ProjectUri,
                    DownloadCount= 1
                });
            }

            await this.downloadsStatisticsCollection.ReplaceOneAsync(repositoryFilter, statistics);

            return statistics;

        }

        private async Task<DownloadStatistics> FindOneOrCreateNewAsync(ObjectId repositoryId, FilterDefinition<DownloadStatistics> repositoryFilter)
        {
            FindOneAndUpdateOptions<DownloadStatistics, DownloadStatistics> options =
                new FindOneAndUpdateOptions<DownloadStatistics, DownloadStatistics>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

            UpdateDefinition<DownloadStatistics> updateDef = new UpdateDefinitionBuilder<DownloadStatistics>()
                    .SetOnInsert(x => x.RepositoryId, repositoryId)
                ;
            try
            {
                return await this.downloadsStatisticsCollection.FindOneAndUpdateAsync(RepoCatFilterBuilder.BuildStatisticsRepositoryFilter(repositoryId), updateDef, options)
                    .ConfigureAwait(false);
            }
            catch (MongoException)
            {
                //upsert might require a retry
                //https://docs.mongodb.com/manual/reference/method/db.collection.findAndModify/#upsert-and-unique-index
                //https://stackoverflow.com/questions/42752646/async-update-or-insert-mongodb-documents-using-net-driver
                return await this.downloadsStatisticsCollection.FindOneAndUpdateAsync(repositoryFilter, updateDef, options)
                    .ConfigureAwait(false);
            }
        }

        private async Task<SearchStatistics> FindOneOrCreateNewAsync(RepositoryQueryParameter repositoryParameter, FilterDefinition<SearchStatistics> repoNameFilter)
        {
            FindOneAndUpdateOptions<SearchStatistics, SearchStatistics> options =
                new FindOneAndUpdateOptions<SearchStatistics, SearchStatistics>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                };

            UpdateDefinition<SearchStatistics> updateDef = new UpdateDefinitionBuilder<SearchStatistics>()
                    .SetOnInsert(x => x.OrganizationName, repositoryParameter.OrganizationName)
                    .SetOnInsert(x => x.RepositoryName, repositoryParameter.RepositoryName)
                ;

            try
            {
                return await this.searchStatisticsCollection.FindOneAndUpdateAsync(repoNameFilter, updateDef, options)
                    .ConfigureAwait(false);
            }
            catch (MongoException)
            {
                //upsert might require a retry
                //https://docs.mongodb.com/manual/reference/method/db.collection.findAndModify/#upsert-and-unique-index
                //https://stackoverflow.com/questions/42752646/async-update-or-insert-mongodb-documents-using-net-driver
                return await this.searchStatisticsCollection.FindOneAndUpdateAsync(repoNameFilter, updateDef, options)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get download statistics for a given repository
        /// </summary>
        /// <param name="repositoryId"></param>
        /// <returns></returns>
        public async Task<DownloadStatistics> GetDownloadStatistics(string repositoryId)
        {
            void CheckParams()
            {
                if (repositoryId == null) throw new ArgumentNullException(nameof(repositoryId));
            }

            CheckParams();

            FilterDefinition<DownloadStatistics> repositoryFilter =
                RepoCatFilterBuilder.BuildStatisticsRepositoryFilter(ObjectId.Parse(repositoryId));

            return await this.FindOneOrCreateNewAsync(ObjectId.Parse(repositoryId), repositoryFilter);
        }
    }
}