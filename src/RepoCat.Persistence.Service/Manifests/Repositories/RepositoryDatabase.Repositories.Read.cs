﻿// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.Repositories.Read.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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

        /// <summary>
        /// Gets all the repositories names
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns> 
        public Task<IAsyncCursor<RepositoryInfo>> GetAllRepositories()
        {
            return this.repositories.FindAsync(FilterDefinition<RepositoryInfo>.Empty);
        }

        /// <summary>
        /// Gets all repositories from a give organization
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public Task<IAsyncCursor<RepositoryInfo>> GetAllRepositories(string organizationName)
        {
            return this.repositories.FindAsync(RepoCatFilterBuilder.BuildRepositoryFilter(organizationName));
        }


        /// <summary>
        /// Gets a repository by the specified name and organization name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public async Task<RepositoryInfo> GetRepository(string organizationName, string repositoryName)
        {
            FilterDefinition<RepositoryInfo> repoFilter = RepoCatFilterBuilder.BuildRepositoryFilter(organizationName, repositoryName);

            RepositoryInfo repo = await (await this.repositories.FindAsync(repoFilter).ConfigureAwait(false))
                .FirstOrDefaultAsync().ConfigureAwait(false);
            return repo;
        }

        /// <summary>
        /// Gets a repository by its ID
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<RepositoryInfo> GetRepositoryById(ObjectId objectId)
        {
            FilterDefinition<RepositoryInfo> repoFilter =
                Builders<RepositoryInfo>.Filter.Where(x =>
                    x.Id == objectId
                );

            RepositoryInfo repo = await (await this.repositories.FindAsync(repoFilter).ConfigureAwait(false))
                .SingleOrDefaultAsync().ConfigureAwait(false);
            return repo;
        }

       
    }
}