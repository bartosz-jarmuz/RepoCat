// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.Repositories.Read.cs" company="bartosz.jarmuz@gmail.com">
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
    public partial class RepositoryDatabase
    {

      

        /// <summary>
        /// Gets all the repositories names
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns> 
        public Task<IAsyncCursor<RepositoryInfo>> GetAllSnapshotRepositories()
        {
            return this.repositories.FindAsync(new FilterDefinitionBuilder<RepositoryInfo>().Where(x=>x.RepositoryMode == RepositoryMode.Snapshot));
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
        /// Gets all the repositories names
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns> 
        public Task<IAsyncCursor<RepositoryInfo>> GetAllRepositories()
        {
            return this.repositories.FindAsync(FilterDefinition<RepositoryInfo>.Empty);
        }

        /// <summary>
        /// Gets all repositories matching search params
        /// </summary>
        /// <param name="repositoryParams">List of key value pairs - Organization and Repository name</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        public async Task<IEnumerable<RepositoryInfo>> GetRepositories(IEnumerable<RepositoryQueryParameter> repositoryParams)
        {
            void CheckParams()
            {
                if (repositoryParams == null) throw new ArgumentNullException(nameof(repositoryParams));
            }

            CheckParams();

            List<Task<RepositoryInfo>> tasks = new List<Task<RepositoryInfo>>();
            var paramsList = repositoryParams.ToList();
            if (paramsList.Any(x => x.OrganizationName == "*"))
            {
                IAsyncCursor<RepositoryInfo> cursor = await this.GetAllRepositories();
                await cursor.ForEachAsync(t => tasks.Add(Task.FromResult(t)));
            }
            else
            {
                foreach (IGrouping<string, RepositoryQueryParameter> groupedParams in paramsList.GroupBy(x => x.OrganizationName))
                {
                    if (groupedParams.Any(x => x.RepositoryName == "*"))
                    {
                        IAsyncCursor<RepositoryInfo> cursor = await this.GetAllRepositories(groupedParams.Key);
                        await cursor.ForEachAsync(t => tasks.Add(Task.FromResult(t)));
                    }
                    else
                    {
                        foreach (RepositoryQueryParameter repositoryParam in groupedParams)
                        {
                            tasks.Add(this.GetRepository(repositoryParam.OrganizationName, repositoryParam.RepositoryName));
                        }
                    }
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.Select(x => x.Result);
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