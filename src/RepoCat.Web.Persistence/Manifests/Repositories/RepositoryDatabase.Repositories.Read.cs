using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// Gets all the repositories names
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns> 
        public Task<IAsyncCursor<RepositoryInfo>> GetAllRepositories()
        {
            return this.repositories.FindAsync(FilterDefinition<RepositoryInfo>.Empty);
        }

        /// <summary>
        /// Gets repositories grouped per organization name
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<RepositoryGrouping>> GetAllRepositoriesGrouped()
        {
            var repos = await this.GetAllRepositories().ConfigureAwait(false);
            return RepositoryGrouping.CreateGroupings(repos.ToList()).ToList().AsReadOnly();
        }


        /// <summary>
        /// Gets a repository by the specified name and organization name
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public async Task<RepositoryInfo> GetRepository(string organizationName, string repositoryName)
        {
            FilterDefinition<RepositoryInfo> repoFilter = BuildRepositoryFilter(organizationName, repositoryName);

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