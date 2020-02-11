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
                    project= await this.database.Create(mappedProject).ConfigureAwait(false);
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

        public async Task<List<string>> GetStamps(RepositoryQueryParameter repositoryQueryParameter)
        {
            var repo = await this.database.GetRepository(repositoryQueryParameter.OrganizationName,repositoryQueryParameter.RepositoryName);
            return await this.database.GetStamps(repo);

        }
        public  Task<ManifestQueryResult> GetAllCurrentProjects(RepositoryQueryParameter repositoryQueryParameter)
        {
            return this.GetCurrentProjects(repositoryQueryParameter, "*", false);
        }

        public Task<ManifestQueryResult> GetAllCurrentProjects(List<RepositoryQueryParameter> repoParams)
        {
            return this.GetCurrentProjects(repoParams, "*", false);
        }

        public Task<ManifestQueryResult> GetCurrentProjects(RepositoryQueryParameter repositoryQueryParameter, string query, bool isRegex)
        {
            return this.GetCurrentProjects(new List<RepositoryQueryParameter>() { repositoryQueryParameter }, query, isRegex);
        }

        public async Task<ManifestQueryResult> GetCurrentProjects(IReadOnlyCollection<RepositoryQueryParameter> repoParams, string query, bool isRegex)
        {
            var sw = Stopwatch.StartNew();
            var projects = await this.database.GetCurrentProjects(this.mapper.Map<IReadOnlyCollection<Persistence.Models.RepositoryQueryParameter>>(repoParams), query, isRegex).ConfigureAwait(false);
            sw.Stop();
            return new ManifestQueryResult(repoParams, projects, sw.Elapsed, query, isRegex);
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
            var repos = await this.GetAllRepositories().ConfigureAwait(false);
            return RepositoryGrouping.CreateGroupings(repos.ToList()).ToList().AsReadOnly();
        }

    }
}
