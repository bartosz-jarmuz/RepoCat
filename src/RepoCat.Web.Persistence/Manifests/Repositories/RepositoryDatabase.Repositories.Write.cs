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
        /// Creates a new repository
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public RepositoryInfo Create(RepositoryInfo info)
        {
            this.repositories.InsertOne(info);
            return info;
        }

        /// <summary>
        /// Updates a repository if it exists (replace entire document). Otherwise, creates new.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public Task<ReplaceOneResult> UpsertReplace(RepositoryInfo info)
        {
            Task<ReplaceOneResult> result = this.repositories.ReplaceOneAsync(x => x.Id == info.Id, info, new ReplaceOptions()
            {
                IsUpsert = true
            });
            return result;
        }


        /// <summary>
        /// Adds a repository if it did not exist. 
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Do not use StringComparison in these filter expressions")]
        public async Task<RepositoryInfo> UpsertUpdate(RepositoryInfo repositoryInfo)
        {
            if (repositoryInfo == null) throw new ArgumentNullException(nameof(repositoryInfo));

            FilterDefinition<RepositoryInfo> repoNameFilter =
                Builders<RepositoryInfo>.Filter.Where(x => 
                    x.RepositoryName.ToUpperInvariant() == repositoryInfo.RepositoryName.ToUpperInvariant()
                    && x.OrganizationName.ToUpperInvariant() == repositoryInfo.OrganizationName.ToUpperInvariant()
                    );

            FindOneAndUpdateOptions<RepositoryInfo, RepositoryInfo> options = new FindOneAndUpdateOptions<RepositoryInfo, RepositoryInfo>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            UpdateDefinition<RepositoryInfo> updateDef = new UpdateDefinitionBuilder<RepositoryInfo>()
                .SetOnInsert(x=>x.OrganizationName, repositoryInfo.OrganizationName)
                .SetOnInsert(x=>x.RepositoryName, repositoryInfo.RepositoryName)
                .SetOnInsert(x=>x.RepositoryMode, repositoryInfo.RepositoryMode)
                ;

            return await this.repositories.FindOneAndUpdateAsync(repoNameFilter, updateDef, options).ConfigureAwait(false);
        }

    
     

    }
}