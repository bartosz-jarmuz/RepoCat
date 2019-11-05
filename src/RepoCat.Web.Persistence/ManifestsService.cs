using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    public class ManifestsService
    {
        private readonly IMongoCollection<ProjectInfo> manifests;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestsService"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public ManifestsService(IRepoCatDbSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.manifests = database.GetCollection<ProjectInfo>(settings.ManifestsCollectionName);
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            var keys = Builders<ProjectInfo>.IndexKeys
                .Text("Components.Tags");
            //.Text(m=>m.Repo);


            var indexModel = new CreateIndexModel<ProjectInfo>(keys);
            this.manifests.Indexes.CreateOne(indexModel);
        }

        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>List&lt;ProjectInfo&gt;.</returns>
        public List<ProjectInfo> Get()
        {
            return this.manifests.Find(manifest => true).ToList();
        }

        /// <summary>
        /// Gets all the repositories names
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        public async Task<List<string>> GetRepositoryNames()
        {
            IAsyncCursor<string> result =
                await this.manifests.DistinctAsync(x => x.RepositoryName, FilterDefinition<ProjectInfo>.Empty).ConfigureAwait(false);
            return await result.ToListAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// Gets all projects for the latest version of a given repository.
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicates do not support StringComparison enum")]
        public async Task<ManifestQueryResult> GetAllCurrentProjects(string repositoryName)
        {
            var stopwatch = Stopwatch.StartNew();
            FilterDefinition<ProjectInfo> repoNameFilter =
                Builders<ProjectInfo>.Filter.Where(x => x.RepositoryName.ToUpperInvariant().Contains(repositoryName.ToUpperInvariant()));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepositoryStamp, repoNameFilter).ConfigureAwait(false))
                .ToListAsync().ConfigureAwait(false);
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectInfo> filter =
                repoNameFilter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == newestStamp);
            var list = await (await this.manifests.FindAsync(filter).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
            stopwatch.Stop();

            return new ManifestQueryResult()
            {
                RepositoryStamp = newestStamp,
                Elapsed = stopwatch.Elapsed,
                ProjectInfos = list
            };
        }


        /// <summary>
        /// Gets all projects for the latest version of a given repository matching specified search parameters
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="query">The string to search by</param>
        /// <param name="isRegex">Specify whether the search string is a Regex</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicate building does not support StringComparison enum")]
        public async Task<ManifestQueryResult> GetCurrentProjects(string repositoryName, string query, bool isRegex)
        {
            var stopwatch = Stopwatch.StartNew();
            FilterDefinition<ProjectInfo> repoNameFilter =
                Builders<ProjectInfo>.Filter.Where(x => x.RepositoryName.ToUpperInvariant().Contains(repositoryName.ToUpperInvariant()));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepositoryStamp, repoNameFilter).ConfigureAwait(false))
                .ToListAsync().ConfigureAwait(false);
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectInfo> filter =
                repoNameFilter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == newestStamp);
            if (!string.IsNullOrEmpty(query))
            {
                filter = filter & BuildTextQuery(query, isRegex);
            }

            var list = await (await this.manifests.FindAsync(filter).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);

            stopwatch.Stop();
            return new ManifestQueryResult()
            {
                RepositoryName = repositoryName,
                RepositoryStamp = newestStamp,
                Elapsed = stopwatch.Elapsed,
                ProjectInfos = list,
                IsRegex = isRegex,
                QueryString = query
            };
        }

        /// <summary>
        /// Gets the item with specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        public ProjectInfo Get(string id)
        {
            return this.manifests.Find<ProjectInfo>(manifest => manifest.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>ProjectInfo.</returns>
        public ProjectInfo Create(ProjectInfo info)
        {
            this.manifests.InsertOne(info);
            return info;
        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="info">The information.</param>
        public void Update(string id, ProjectInfo info)
        {
            this.manifests.ReplaceOne(manifest => manifest.Id == id, info);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="info">The information.</param>
        public void Remove(ProjectInfo info)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == info.Id);
        }

        /// <summary>
        /// Removes the item with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == id);
        }

        private static FilterDefinition<ProjectInfo> BuildTextQuery(string query, bool isRegex)
        {
            if (!isRegex)
            {
                return Builders<ProjectInfo>.Filter.Text(query);
            }
            else
            {
                var regex = new BsonRegularExpression(new System.Text.RegularExpressions.Regex(query,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                return Builders<ProjectInfo>.Filter.Regex(GetComponentFieldName(nameof(ComponentManifest.Tags)),
                           regex)
                       | Builders<ProjectInfo>.Filter.Regex(GetComponentFieldName(nameof(ComponentManifest.Name)),
                           regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.AssemblyName, regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.ProjectName, regex)
                       | Builders<ProjectInfo>.Filter.Regex(x => x.TargetExtension, regex);
            }
        }

        private static string GetComponentFieldName(string field)
        {
            return nameof(ProjectInfo.Components) + "." + field;
        }


       
    }
}