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
        /// Creates a new repository
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public Task<ReplaceOneResult> Replace(RepositoryInfo info)
        {
            Task<ReplaceOneResult> result = this.repositories.ReplaceOneAsync(x => x.Id == info.Id, info, new ReplaceOptions(){ IsUpsert = true});
            return result;
        }


        /// <summary>
        /// Gets a repository by name
        /// </summary>
        /// <returns>Task&lt;List&lt;System.String&gt;&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Do not use StringComparison in these filter expressions")]
        public async Task<RepositoryInfo> Upsert(string organizationName, string repositoryName)
        {
            FilterDefinition<RepositoryInfo> repoNameFilter =
                Builders<RepositoryInfo>.Filter.Where(x => 
                    x.RepositoryName.ToUpperInvariant() == repositoryName.ToUpperInvariant()
                    && x.OrganizationName.ToUpperInvariant() == organizationName.ToUpperInvariant()
                    );

            FindOneAndUpdateOptions<RepositoryInfo, RepositoryInfo> options = new FindOneAndUpdateOptions<RepositoryInfo, RepositoryInfo>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            UpdateDefinition<RepositoryInfo> updateDef = new UpdateDefinitionBuilder<RepositoryInfo>()
                .Set(x=>x.OrganizationName, organizationName)
                .Set(x=>x.RepositoryName, repositoryName)
                ;

            return await this.repositories.FindOneAndUpdateAsync(repoNameFilter, updateDef, options).ConfigureAwait(false);
        }

    
     

    }
}