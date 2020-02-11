// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RepoCat.Persistence.Models;

namespace RepoCat.Persistence.Service
{
    /// <summary>
    /// Allows access to the stored Manifests data
    /// </summary>
    public partial class RepositoryDatabase
    {
        private readonly IMongoCollection<ProjectInfo> projects;
        private readonly IMongoCollection<RepositoryInfo> repositories;
        private readonly IMongoDatabase database;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryDatabase"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public RepositoryDatabase(IRepoCatDbSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);
            this.projects = this.database.GetCollection<ProjectInfo>(settings.ProjectsCollectionName);
            this.repositories = this.database.GetCollection<RepositoryInfo>(settings.RepositoriesCollectionName);
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            IndexKeysDefinition<ProjectInfo> keys = Builders<ProjectInfo>.IndexKeys
                .Text(x => x.ProjectName)
                .Text(x => x.ProjectDescription)
                .Text(x => x.AssemblyName)
                .Text(x => x.ProjectUri)
                .Text(x=>x.Tags)
                .Text("Properties.Values")
                .Text($"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Name)}")
                .Text($"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Description)}")
                .Text($"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.DocumentationUri)}")
                .Text($"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Tags)}")
                .Text("$**")
                ;
            CreateIndexModel<ProjectInfo> indexModel = new CreateIndexModel<ProjectInfo>(keys, new CreateIndexOptions()
            {
                Weights = new BsonDocument(
                    new Dictionary<string, object>()
                    {
                        //high priority
                        {nameof(ProjectInfo.ProjectName), 10},
                        {nameof(ProjectInfo.AssemblyName), 10},
                        {nameof(ProjectInfo.Tags), 10},

                        { $"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Name)}",10},
                        { $"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Tags)}",10},

                        //medium priority 
                        {nameof(ProjectInfo.ProjectDescription), 5},
                        { $"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.Description)}",5},

                        //low priority 
                        {nameof(ProjectInfo.ProjectUri), 3},
                        {nameof(ProjectInfo.DocumentationUri), 3},
                        {nameof(ProjectInfo.DownloadLocation), 3},
                        { $"{nameof(ProjectInfo.Components)}.{nameof(ComponentManifest.DocumentationUri)}",3},

                        {"$**", 1 }
                    }
                )
            });
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

        /// <summary>
        /// Gets summary info about a collection.
        /// </summary>
        public async Task<IEnumerable<CollectionSummary>> GetSummary()
        {
            var projectsSummary = await this.database.RunCommandAsync<CollectionSummary>("{collstats: '"+this.projects.CollectionNamespace.CollectionName+"'}").ConfigureAwait(false);
            var reposSummary = await this.database.RunCommandAsync<CollectionSummary>("{collstats: '"+this.repositories.CollectionNamespace.CollectionName+"'}").ConfigureAwait(false);

            return new List<CollectionSummary>() {projectsSummary, reposSummary};
        }
    }
}