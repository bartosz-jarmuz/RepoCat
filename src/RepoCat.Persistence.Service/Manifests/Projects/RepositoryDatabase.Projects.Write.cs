// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.Projects.Write.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
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
        /// Creates a new item.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>ProjectInfo.</returns>
        public async Task<ProjectInfo> Create(ProjectInfo info)
        {
            void CheckArgs()
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
            }

            CheckArgs();

            EnsureRepoStampIsSet(info);

            await this.projects.InsertOneAsync(info).ConfigureAwait(false);
            return info;
        }

        private static void EnsureRepoStampIsSet(ProjectInfo info)
        {
            if (string.IsNullOrEmpty(info.RepositoryStamp))
            {
                info.RepositoryStamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Creates the entity if it didn't exist, or just updates it
        /// </summary>
        /// <param name="prjInfo"></param>
        /// <returns></returns>
        public async Task<ProjectInfo> Upsert(ProjectInfo prjInfo)
        {
            void CheckArgs()
            {
                if (prjInfo == null) throw new ArgumentNullException(nameof(prjInfo));
            }

            CheckArgs();

            EnsureRepoStampIsSet(prjInfo);

            FilterDefinition<ProjectInfo> repoNameFilter =
                Builders<ProjectInfo>.Filter.Where(x =>
                    x.RepositoryId == prjInfo.RepositoryId
                    && x.ProjectName == prjInfo.ProjectName
                    && x.ProjectUri == prjInfo.ProjectUri
                );

            var options = new FindOneAndReplaceOptions<ProjectInfo, ProjectInfo>()
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };
            try
            {
               return await this.projects.FindOneAndReplaceAsync(repoNameFilter, prjInfo, options).ConfigureAwait(false);

            }
            catch (MongoException)
            {
                //upsert might require a retry
                //https://docs.mongodb.com/manual/reference/method/db.collection.findAndModify/#upsert-and-unique-index
                //https://stackoverflow.com/questions/42752646/async-update-or-insert-mongodb-documents-using-net-driver
                return await this.projects.FindOneAndReplaceAsync(repoNameFilter, prjInfo, options).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="info">The information.</param>
        public void Update(string id, ProjectInfo info)
        {
            this.projects.ReplaceOne(manifest => manifest.Id == ObjectId.Parse(id), info);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="info">The information.</param>
        public void Remove(ProjectInfo info)
        {
            this.projects.DeleteOne(manifest => manifest.Id == info.Id);
        }

        /// <summary>
        /// Removes the item with specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id)
        {
            this.projects.DeleteOne(manifest => manifest.Id == ObjectId.Parse(id));
        }

    }
}