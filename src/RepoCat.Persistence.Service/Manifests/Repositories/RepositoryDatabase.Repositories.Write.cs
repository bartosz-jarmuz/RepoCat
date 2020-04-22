// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.Repositories.Write.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
        [SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Do not use StringComparison in these filter expressions")]
        
        public async Task<RepositoryInfo> UpsertUpdate(RepositoryInfo repositoryInfo)
        {
            void CheckArgs()
            {
                if (repositoryInfo == null) throw new ArgumentNullException(nameof(repositoryInfo));
            }

            CheckArgs();

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
            try
            {
                return await this.repositories.FindOneAndUpdateAsync(repoNameFilter, updateDef, options)
                    .ConfigureAwait(false);
            }
            catch (MongoException)
            {
                //upsert might require a retry
                //https://docs.mongodb.com/manual/reference/method/db.collection.findAndModify/#upsert-and-unique-index
                //https://stackoverflow.com/questions/42752646/async-update-or-insert-mongodb-documents-using-net-driver
                return await this.repositories.FindOneAndUpdateAsync(repoNameFilter, updateDef, options)
                    .ConfigureAwait(false);
            }
        }

    
     

    }
}