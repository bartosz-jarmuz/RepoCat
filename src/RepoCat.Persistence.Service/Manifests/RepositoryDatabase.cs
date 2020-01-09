﻿using System;
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



      
    }
}