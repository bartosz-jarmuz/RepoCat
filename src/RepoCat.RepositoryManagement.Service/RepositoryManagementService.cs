// -----------------------------------------------------------------------
//  <copyright file="RepositoryManagementService.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ApplicationInsights;
using MongoDB.Driver;
using RepoCat.Persistence.Models;
using RepoCat.Persistence.Service;

namespace RepoCat.RepositoryManagement.Service
{
    public class RepositoryManagementService : IRepositoryManagementService
    {
        private readonly RepositoryDatabase database;
        private readonly IMapper mapper;
        private readonly TelemetryClient telemetryClient;

        public RepositoryManagementService(RepositoryDatabase database, IMapper mapper, TelemetryClient telemetryClient)
        {
            this.database = database;
            this.mapper = mapper;
            this.telemetryClient = telemetryClient;
        }

        public async Task<ProjectInfo> Upsert(Transmission.Models.ProjectInfo projectInfo)
        {
            ProjectInfo mappedProject;
            RepositoryInfo mappedRepo;
            try
            {
                mappedRepo = this.mapper.Map<RepositoryInfo>(projectInfo.RepositoryInfo);
                mappedProject = this.mapper.Map<ProjectInfo>(projectInfo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Mapping exception", ex);
            }
          

            try
            {
                RepositoryInfo repo = await this.database.UpsertUpdate(mappedRepo).ConfigureAwait(false);
                this.telemetryClient.TrackUpserted(repo);

                mappedProject.RepositoryId = repo.Id;
                ProjectInfo project;
                if (repo.RepositoryMode == RepositoryMode.Snapshot)
                {
                    project= await this.database. Create(mappedProject).ConfigureAwait(false);
                }
                else
                {
                    project = await this.database.Upsert(mappedProject).ConfigureAwait(false);
                }

                this.telemetryClient.TrackUpserted(project);
                return project;

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Database write exception", ex);
            }
        }

        public Task<ProjectInfo> GetById(string id)
        {
            return this.database.GetById(id);
        }

        public async Task<IEnumerable<CollectionSummary>> GetSummary()
        {
            return await this.database.GetSummary().ConfigureAwait(false);
        }

        public async Task<List<string>> GetStamps(RepositoryInfo repositoryInfo)
        {
            return await this.database.GetStamps(repositoryInfo);

        }
        public  Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryInfo repositoryInfo)
        {
            return this.GetCurrentProjects(repositoryInfo, "*", false);
        }

        public Task<ManifestQueryResult> GetAllCurrentProjects(List<RepositoryInfo> repoParams)
        {
            return this.GetCurrentProjects( repoParams, "*", false);
        }

        public Task<ManifestQueryResult> GetCurrentProjects(RepositoryInfo repositoryInfo, string query, bool isRegex)
        {
            return this.GetCurrentProjects(new List<RepositoryInfo>() { repositoryInfo }, query, isRegex);
        }

        public async Task<ManifestQueryResult> GetCurrentProjects(IReadOnlyCollection<RepositoryInfo> repositories, string query, bool isRegex)
        {
            Stopwatch sw = Stopwatch.StartNew();
            IEnumerable<Project> projects = await this.database.GetCurrentProjects(repositories, query, isRegex).ConfigureAwait(false);
            sw.Stop();
            return new ManifestQueryResult(repositories, projects, sw.Elapsed, query, isRegex);
        }

        public async Task<IEnumerable<RepositoryInfo>> GetRepositories(IReadOnlyCollection<RepositoryQueryParameter> repoParams)
        {
           return await this.database.GetRepositories(this.mapper.Map<IReadOnlyCollection<Persistence.Models.RepositoryQueryParameter>>(repoParams)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RepositoryInfo>> GetRepositories(RepositoryQueryParameter repoParam)
        {
            return await this.database.GetRepositories(this.mapper.Map<IReadOnlyCollection<Persistence.Models.RepositoryQueryParameter>>(new List<RepositoryQueryParameter>(){repoParam})).ConfigureAwait(false);
        }
        
        public async Task<bool> DeleteRepository(RepositoryQueryParameter repoParam)
        {
            RepositoryInfo repo = await this.database.GetRepository(repoParam.OrganizationName,repoParam.RepositoryName).ConfigureAwait(false);
            if (repo == null)
            {
                throw new InvalidOperationException($"Failed to find repository {repoParam.RepositoryName} in {repoParam.OrganizationName}");
            }

            DeleteResult projectsResult = await this.database.DeleteProjects(repo).ConfigureAwait(false);
            if (projectsResult.IsAcknowledged)
            {
                DeleteResult repoResult = await this.database.DeleteRepository(repo).ConfigureAwait(false);
                if (repoResult.IsAcknowledged)
                {
                    return true;
                }
            }

            return false;
        }

        

        public async Task<IEnumerable<RepositoryInfo>> GetAllRepositories()
        {
            return (await this.database.GetAllRepositories().ConfigureAwait(false)).ToList();
        }

        /// <summary>
        /// Gets repositories grouped per organization name
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<RepositoryGrouping>> GetAllRepositoriesGrouped()
        {
            IEnumerable<RepositoryInfo> repos = await this.GetAllRepositories().ConfigureAwait(false);
            return RepositoryGrouping.CreateGroupings(repos.ToList()).ToList().AsReadOnly();
        }

    }
}
