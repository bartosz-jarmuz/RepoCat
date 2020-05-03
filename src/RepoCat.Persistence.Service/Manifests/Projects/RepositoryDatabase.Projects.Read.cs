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
        /// Gets the coollection of stamps for a given repository
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        public async Task<List<string>> GetStamps(RepositoryInfo repository)
        {
            void CheckParams()
            {
                if (repository == null) throw new ArgumentNullException(nameof(repository));
            }

            CheckParams();

            FilterDefinition<ProjectInfo> filter = Builders<ProjectInfo>.Filter.Eq(p => p.RepositoryId, repository.Id);
            List<string> stamps = await (await this.projects.DistinctAsync(x => x.RepositoryStamp, filter).ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
            return stamps;
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
            List<Task<IEnumerable<Project>>> tasks = new List<Task<IEnumerable<Project>>>();
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
        /// Gets projects matching the query from all repositories in all organizations (both current projects and old shapshots!)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsByQuery(string query, bool isRegex)
        {
            FilterDefinition<ProjectInfo> filter;
            if (isRegex)
            {
                filter = RepoCatFilterBuilder.BuildProjectsTextFilter(query, isRegex);
            }
            else
            {
                filter = RepoCatFilterBuilder.BuildProjectsFuzzyTextFilter(query);
            }
            return this.ExecuteFilter(filter);
        }


        private async Task<IEnumerable<Project>> GetProjects(RepositoryInfo repository, string query, bool isRegex, string stamp = null)
        {
            FilterDefinition<ProjectInfo> filter = await RepoCatFilterBuilder.BuildProjectsFilter(this.projects, query, isRegex, repository, stamp).ConfigureAwait(false);

            return await this.ExecuteFilter(filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all projects
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectInfo>> GetAllProjects()
        {
            return await (await this.projects.FindAsync(new FilterDefinitionBuilder<ProjectInfo>().Empty)
                .ConfigureAwait(false)).ToListAsync().ConfigureAwait(false);
        }


        private async Task<IEnumerable<Project>> ExecuteFilter(FilterDefinition<ProjectInfo> filter)
        {
            bool containsTextFilter = RepoCatFilterBuilder.CheckIfContainsTextFilter(filter);
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
            /// <summary>
            /// 
            /// </summary>
#pragma warning disable S3459 // Unassigned members should be removed
            public List<RepositoryInfo> RepositoryInfo { get; set; }
#pragma warning restore S3459 // Unassigned members should be removed
        }
        
    }
}