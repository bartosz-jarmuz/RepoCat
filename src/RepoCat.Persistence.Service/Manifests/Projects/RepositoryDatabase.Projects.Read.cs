// -----------------------------------------------------------------------
//  <copyright file="RepositoryDatabase.Projects.Read.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        /// Gets the item with specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ProjectInfo.</returns>
        public async Task<ProjectInfo> GetById(string id)
        {
            return await (await this.projects.FindAsync<ProjectInfo>(manifest => manifest.Id == ObjectId.Parse(id)).ConfigureAwait(false)).FirstOrDefaultAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repos"></param>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Project>> GetCurrentProjects(IEnumerable<RepositoryInfo> repos, string query, bool isRegex)
        {
            var tasks = new List<Task<IEnumerable<Project>>>();
            if (repos != null)
            {
                foreach (RepositoryInfo repositoryInfo in repos)
                {
                    tasks.Add(this.GetProjects(repositoryInfo, query, isRegex));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return tasks.SelectMany(x => x.Result);
        }

        /// <summary>
        /// Gets the coollection of stamps for a given repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public async Task<List<string>> GetStamps(RepositoryInfo repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            var filter = Builders<ProjectInfo>.Filter.Eq(p => p.RepositoryId, repository.Id);
            List<string> stamps = await (await this.projects.DistinctAsync(x => x.RepositoryStamp, filter).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
            return stamps;
        }


        /// <summary>
        /// Gets all projects for the latest version of a given repository matching specified search parameters
        /// </summary>
        /// <param name="repositoryParams">List of key value pairs - Organization and Repository name</param>
        /// <param name="query">The string to search by. Set to null, empty or * to ignore the query</param>
        /// <param name="isRegex">Specify whether the search string is a Regex</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        public async Task<IEnumerable<Project>> GetCurrentProjects(IEnumerable<RepositoryQueryParameter> repositoryParams, string query, bool isRegex)
        {
            if (repositoryParams == null) throw new ArgumentNullException(nameof(repositoryParams));

            var tasks = new List<Task<RepositoryInfo>>();
            foreach (RepositoryQueryParameter repositoryParam in repositoryParams)
            {
                 tasks.Add(this.GetRepository(repositoryParam.OrganizationName, repositoryParam.RepositoryName));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return await this.GetCurrentProjects(tasks.Select(x=>x.Result), query, isRegex).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets projects matching the query from all repositories in all organizations (both current projects and old shapshots!)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsByQuery(string query, bool isRegex)
        {
            FilterDefinition<ProjectInfo> filter = RepoCatFilterBuilder.BuildProjectsTextFilter(query, isRegex);
            return this.ExecuteFilter(filter);
        }


        private async Task<IEnumerable<Project>> GetProjects(RepositoryInfo repository, string query, bool isRegex, string stamp = null)
        {
            FilterDefinition<ProjectInfo> filter = await RepoCatFilterBuilder.BuildProjectsFilter(this.projects, query, isRegex, repository, stamp).ConfigureAwait(false);
            return await this.ExecuteFilter(filter).ConfigureAwait(false);
        }
   

        private async Task<IEnumerable<Project>> ExecuteFilter(FilterDefinition<ProjectInfo> filter)
        {
            var containsTextFilter = RepoCatFilterBuilder.CheckIfContainsTextFilter(filter);
            IAggregateFluent<ProjectWithRepos> aggr;
            if (containsTextFilter)
            {

                aggr = this.projects.Aggregate()
                    .Match(filter)
                    .Sort(Builders<ProjectInfo>.Sort.MetaTextScore("textScore"))
                    .Lookup(
                        foreignCollection: this.repositories,
                        localField: prj => prj.RepositoryId,
                        foreignField: repo => repo.Id,
                        @as: (ProjectWithRepos pr) => pr.RepositoryInfo
                    );
            }
            else
            { 
                aggr = this.projects.Aggregate()
                    .Match(filter)
                    .Lookup(
                        foreignCollection: this.repositories,
                        localField: prj => prj.RepositoryId,
                        foreignField: repo => repo.Id,
                        @as: (ProjectWithRepos pr) => pr.RepositoryInfo
                    );
            }

            IEnumerable<Project> projected = (await aggr.ToListAsync().ConfigureAwait(false)).Select(x => new Project()
                {ProjectInfo = x, RepositoryInfo = x.RepositoryInfo.First()});

            return projected;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class ProjectWithRepos : ProjectInfo
        {

            public List<RepositoryInfo> RepositoryInfo { get; set; }
        }

    }
}