﻿using System;
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
        /// Creates a new item.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns>ProjectInfo.</returns>
        public async Task<ProjectInfo> Create(ProjectInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

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
            if (prjInfo == null) throw new ArgumentNullException(nameof(prjInfo));

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
            ProjectInfo result = await this.projects.FindOneAndReplaceAsync(repoNameFilter, prjInfo, options).ConfigureAwait(false);

            return result;
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