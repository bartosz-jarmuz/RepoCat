using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
        /// Gets all items
        /// </summary>
        /// <returns>List&lt;ProjectInfo&gt;.</returns>
        public IFindFluent<ProjectInfo, ProjectInfo> GetAll()
        {
            return this.projects.Find(FilterDefinition<ProjectInfo>.Empty);
        }

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
        /// Gets all projects for the latest version of a given repository.
        /// </summary>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        public Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryInfo repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));

            return this.GetAllCurrentProjects(repository.OrganizationName, repository.RepositoryName);
        }

        /// <summary>
        /// Gets all projects for the latest version of a given repository.
        /// </summary>
        /// <param name="organizationName">Name of the organization which holds the repository</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:Specify StringComparison", Justification = "Predicates do not support StringComparison enum")]
        public async Task<ManifestQueryResult> GetAllCurrentProjects(string organizationName, string repositoryName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            RepositoryInfo repo = await this.GetRepository(organizationName, repositoryName).ConfigureAwait(false);

            FilterDefinition<ProjectInfo> projectFilter =
                Builders<ProjectInfo>.Filter.Where(x => x.RepositoryId.Equals(repo.Id));

            (string newestStamp, FilterDefinition<ProjectInfo> filter) filter = await this.GetNewestStampFilter(projectFilter, repo.RepositoryMode).ConfigureAwait(false);

            List<Project> list = (await this.GetProjects(filter.filter).ConfigureAwait(false)).ToList();

            stopwatch.Stop();

            return new ManifestQueryResult()
            {
                OrganizationName = repo.OrganizationName,
                RepositoryName = repo.RepositoryName,
                RepositoryStamp = filter.newestStamp,
                Elapsed = stopwatch.Elapsed,
                Projects = list
            };
        }

      


        /// <summary>
        /// Gets all projects for the latest version of a given repository matching specified search parameters
        /// </summary>
        /// <param name="organizationName">Name of the organization which holds the repository</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="query">The string to search by</param>
        /// <param name="isRegex">Specify whether the search string is a Regex</param>
        /// <returns>Task&lt;ManifestQueryResult&gt;.</returns>
        public async Task<ManifestQueryResult> GetCurrentProjects(string organizationName, string repositoryName, string query, bool isRegex)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            RepositoryInfo repo = await this.GetRepository(organizationName, repositoryName).ConfigureAwait(false);

            FilterDefinition<ProjectInfo> projectFilter = Builders<ProjectInfo>.Filter.Where(x => x.RepositoryId.Equals(repo.Id));

            (string newestStamp, FilterDefinition<ProjectInfo> filter) filter = await this.GetNewestStampFilter(projectFilter, repo.RepositoryMode).ConfigureAwait(false);


            if (!string.IsNullOrEmpty(query))
            {
                projectFilter = filter.filter & BuildTextFilter(query, isRegex);
            }

            List<Project> list = (await this.GetProjects(projectFilter).ConfigureAwait(false)).ToList();

            stopwatch.Stop();
            return new ManifestQueryResult()
            {
                OrganizationName = repo.OrganizationName,
                RepositoryName = repo.RepositoryName,
                RepositoryStamp = filter.newestStamp,
                Elapsed = stopwatch.Elapsed,
                Projects = list,
                IsRegex = isRegex,
                QueryString = query
            };
        }

        private async Task<(string newestStamp, FilterDefinition<ProjectInfo> filter)> GetNewestStampFilter(
            FilterDefinition<ProjectInfo> projectFilter, RepositoryMode repositoryMode)
        {
            List<string> stamps =
                await (await this.projects.DistinctAsync(x => x.RepositoryStamp, projectFilter).ConfigureAwait(false))
                    .ToListAsync().ConfigureAwait(false);
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            if (repositoryMode == RepositoryMode.Snapshot)
            {
                projectFilter = projectFilter &
                                Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == newestStamp);
            }

            return (newestStamp, projectFilter);
        }


        /// <summary>
        /// Gets projects matching the query from all repositories in all organizations
        /// </summary>
        /// <param name="query"></param>
        /// <param name="isRegex"></param>
        /// <returns></returns>
        public Task<IEnumerable<Project>> GetProjectsByQuery(string query, bool isRegex)
        {
            FilterDefinition<ProjectInfo> filter = BuildTextFilter(query, isRegex);
                
            return this.GetProjects(filter);
        }

        /// <summary>
        /// Gets projects matching the query from all repositories in all organizations
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<Project>> GetProjects(FilterDefinition<ProjectInfo> filter)
        {
            IAggregateFluent<ProjectWithRepos> aggr = this.projects.Aggregate()
                .Match(filter)
                .Lookup(
                    foreignCollection: this.repositories,
                    localField: prj => prj.RepositoryId,
                    foreignField: repo => repo.Id,
                    @as: (ProjectWithRepos pr) => pr.RepositoryInfo
                );

            IEnumerable<Project> projected = (await aggr.ToListAsync().ConfigureAwait(false)).Select(x => new Project()
                { ProjectInfo = x, RepositoryInfo = x.RepositoryInfo.First() });

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