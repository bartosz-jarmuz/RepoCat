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
    public partial class RepositoryDatabase
    {
        private readonly IMongoCollection<ProjectInfo> projects;
        private readonly IMongoCollection<RepositoryInfo> repositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDatabase"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public RepositoryDatabase(IRepoCatDbSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.projects = database.GetCollection<ProjectInfo>(settings.ProjectsCollectionName);
            this.repositories = database.GetCollection<RepositoryInfo>(settings.RepositoriesCollectionName);
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            IndexKeysDefinition<ProjectInfo> keys = Builders<ProjectInfo>.IndexKeys
                    .Text($"$**");

            CreateIndexModel<ProjectInfo> indexModel = new CreateIndexModel<ProjectInfo>(keys);
            
            this.projects.Indexes.CreateOne(indexModel);

            this.repositories.Indexes.CreateOne(
                new CreateIndexModel<RepositoryInfo>(
                    Builders<RepositoryInfo>.IndexKeys
                        .Ascending(r => r.OrganizationName)
                        .Ascending(r => r.RepositoryName), new CreateIndexOptions()
                    {
                        Sparse = true,
                        Unique = true
                    }
                )
            );
        }


        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicate building does not support StringComparison enum")]
        private static FilterDefinition<RepositoryInfo> BuildRepositoryFilter(string organizationName, string repositoryName)
        {
            FilterDefinition<RepositoryInfo> repoNameFilter =
                Builders<RepositoryInfo>.Filter.Where(x =>
                    x.RepositoryName.ToUpperInvariant() == repositoryName.ToUpperInvariant()
                    && x.OrganizationName.ToUpperInvariant() == organizationName.ToUpperInvariant()
                );
            return repoNameFilter;
        }


        private static FilterDefinition<ProjectInfo> BuildTextFilter(string query, bool isRegex)
        {
            if (!isRegex)
            {
                return Builders<ProjectInfo>.Filter.Text(query);
            }
            else
            {
                BsonRegularExpression regex = new BsonRegularExpression(new System.Text.RegularExpressions.Regex(query,
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