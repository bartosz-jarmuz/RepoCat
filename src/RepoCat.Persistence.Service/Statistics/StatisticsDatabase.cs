using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RepoCat.Persistence.Models;
using RepoCat.Utilities;

namespace RepoCat.Persistence.Service
{
    /// <summary>
    /// Allows access to the stored Manifests data
    /// </summary>
    public partial class StatisticsDatabase
    {
        private readonly IMongoCollection<SearchStatistics> searchStatisticsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDatabase"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public StatisticsDatabase(IRepoCatDbSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.searchStatisticsCollection = database.GetCollection<SearchStatistics>(settings.SearchStatisticsCollectionName);
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
            if (repositoryParameter == null) throw new ArgumentNullException(nameof(repositoryParameter));

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
            if (repositoryParameter == null) throw new ArgumentNullException(nameof(repositoryParameter));

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

            ReplaceOneResult result = await this.searchStatisticsCollection.ReplaceOneAsync(repoNameFilter, statistics);

            return statistics;
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
    }
}